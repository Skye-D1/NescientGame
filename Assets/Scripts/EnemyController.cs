using UnityEngine;

//Name: Skye Drury
//File: EnemyController.cs
//Purpose: Manage enemy movement, receiving and communicating noises, and other enemy behavior

public class EnemyController : MonoBehaviour
{
    [SerializeField] Vector2 target; // target movement location
    float moveSpeed = 400f; // speed of movement
    float tempMoveSpeed; // temporary movespeed per frame
    Vector3 movement = new Vector3(); // current movement direction
    [SerializeField] bool targetPriority; // whether the target is high priority (player noise)
    [SerializeField] bool hasTarget; // whether current target is valid
    bool didTargetUpdate; // whether the enemy has updated target this frame
    float overshootTimer; // time left moving past target
    public float echoRadius; // radius to echo recieved noise to others
    LayerMask enemyMask; // layermask for finding other enemies
    Collider2D[] enemiesFound; // enemies found wjen circle
    public bool isStatic; // if enemy does not move for tutorial
    public bool isLeader; // if enemy will create wander targets
    bool isPlant;
    LineRenderer lineRenderer;
    public GameObject staticPlant; // object to replace enemy with
    PlayerController playerScript; // the player controlscript for purposes
    LayerMask levelMask;
    float strafeValue = 5f; // direction and magnitude of strafe movements
    float wanderCooldown = 0f;
    float maxWanderCooldown = 3f;
    float[] mapEdgesXY = {-30f, 30f, -30f, 30f}; // {-70f, 62f, -40, 28};
    float targetRandFactor = 3f;
    float randCooldown = 0f;
    Vector2 randValues;
    Rigidbody2D RB;
    [SerializeField] float targetDecayTimer = 0;
    [SerializeField] float noTargetUpdateTimer = 0;

    Vector2 preciseTarget; // exact target location
    Vector2 prevPreciseTarget; // exact target location last frame
    [SerializeField] float predictiveValue; // how much this individual predicts target position
    Animator anim;
    SpriteRenderer selfRenderer;
    float flipCooldown;
    float destroyTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyMask = LayerMask.GetMask("Enemy");
        levelMask = LayerMask.GetMask("Default");
        target = new Vector2(transform.position.x, transform.position.y); // target own position on start
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        RB = gameObject.GetComponent<Rigidbody2D>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        predictiveValue = Random.Range(0.5f, 3f);
        selfRenderer = gameObject.GetComponent<SpriteRenderer>();
        
        // determine if leader
        if (Random.value > 0.4) { // todo: check proximity for other leaders
            isLeader = true;
            lineRenderer.startColor = new Color(1f,1f,0f,1f);
        }

        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlant){
            RB.linearVelocity = new Vector3();
            destroyTimer -= Time.deltaTime;
            if(destroyTimer <= 0){
                playerScript.gameScore += 5f;
                playerScript.deltaScoreOpacity = 1f;
                playerScript.deltaScoreDisplay.text = "+5";
                Destroy(gameObject);
            }
        } else{
            didTargetUpdate = false; // allow one target update per frame
            flipCooldown -= Time.deltaTime;
            if (flipCooldown <= 0 && ((movement.x > 0.2f && !selfRenderer.flipX) || (movement.x < -0.2f && selfRenderer.flipX))) {
                if (movement.x > 0.2f) {
                    selfRenderer.flipX = true;
                    flipCooldown = 0.5f;
                } else if (movement.x < -0.2f) {
                    selfRenderer.flipX = false;
                    flipCooldown = 0.5f;
                }
            }

            // update noTargetUpdateTimer
            if (hasTarget && targetPriority) {
                noTargetUpdateTimer += Time.deltaTime;

                // if no target update for a while, ignore predictive and offset targeting and go to precise location
                if (noTargetUpdateTimer > 5f) {
                    target = preciseTarget;
                    prevPreciseTarget = preciseTarget;
                    noTargetUpdateTimer = 0;
                }
            }
            
            // start moving past target for a time when reached
            if (targetPriority && Vector2.Distance(target, new Vector2(transform.position.x, transform.position.y)) < 1f && overshootTimer <= 0 && movement.magnitude > 0) {
                overshootTimer = 3f;
                hasTarget = false;
            } else if ((Vector2.Distance(target, new Vector2(transform.position.x, transform.position.y)) < 1f) || (!targetPriority && Vector2.Distance(target, new Vector2(transform.position.x, transform.position.y)) < 5f)) {
                hasTarget = false;  
            }

            // target decay to forget target after some time
            if (targetDecayTimer < 22f) {
                targetDecayTimer += Time.deltaTime;
            } else {
                hasTarget = false;
            }

            // keep moving past target for a bit (overshoot)
            if (overshootTimer > 0) {
                overshootTimer -= Time.deltaTime;
                lineRenderer.enabled = false;
                target = new Vector2(transform.position.x, transform.position.y);
                targetPriority = false;
            // move towards target
            } else if (!isStatic && hasTarget) {
                tempMoveSpeed = moveSpeed;
                if (prevPreciseTarget.magnitude > 0 && targetPriority) { // if predictive targeting
                    // higher rand value (further targeting) will cause temporary move faster to surround player
                    tempMoveSpeed += (moveSpeed * 0.8f) * (randValues.magnitude / targetRandFactor);
                }

                movement = Vector3.Normalize(new Vector3(target.x, target.y, 0) - transform.position) * tempMoveSpeed;
                if (playerScript.debugLasers) { // laser when laser time
                    lineRenderer.enabled = true;
                } else {
                    lineRenderer.enabled = false; // not laser when not laser time
                }
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, new Vector3(target.x, target.y, transform.position.z));
            }

            // strafe around obstacles
            if (hasTarget && Vector2.Distance(target, transform.position) > 2f) {
                if (RB.linearVelocity.magnitude < 0.05f) {
                    Vector2 newMovement = Vector2.Perpendicular(new Vector2(movement.x, movement.y)) * strafeValue;
                    movement = new Vector3(newMovement.x, newMovement.y, 0);
                }
            }

            // do movement
            if (!isStatic && hasTarget && (movement * Time.deltaTime).magnitude < 99999f) {
                RB.AddForce(movement * Time.deltaTime);
            } else if ((movement * Time.deltaTime).magnitude >= 99999f) {
                Debug.Log("debug: no don't do that;" + transform.position);
            }

            // no fast
            if (RB.linearVelocity.magnitude > 5f) {
                Debug.Log("debug: speeding ticket issued.");
                RB.linearVelocity = new Vector2();
            }

            // leader generate new wander point if necessary
            if (isLeader && !hasTarget) {
                if (wanderCooldown < 0) {
                    // generate new wander point and broadcast to nearby
                    Vector2 newPoint;
                    int loopys = 0;
                    do {
                        loopys++;
                        newPoint = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * 25f;
                    } while (loopys < 10 && ((newPoint.x + transform.position.x) < mapEdgesXY[0] || (newPoint.x + transform.position.x) > mapEdgesXY[1] || (newPoint.y + transform.position.y) < mapEdgesXY[2] || (newPoint.y + transform.position.y) > mapEdgesXY[3]));
                    if (loopys > 9) {
                        //Debug.Log("loopys problem 1 in enemycontroller");
                        hasTarget = false;
                    }
                    echoNoise(new Vector2(transform.position.x, transform.position.y) + newPoint, false, 10f);
                    target = new Vector2(transform.position.x, transform.position.y) + newPoint;
                    noTargetUpdateTimer = 0;
                    targetDecayTimer = 0;
                    //Debug.Log("wander point broadcasted " + newPoint + " | " + transform.position);
                    wanderCooldown = maxWanderCooldown + (maxWanderCooldown * (Random.value - 0.5f));
                    didTargetUpdate = true;
                } else {
                    wanderCooldown -= Time.deltaTime;
                }
            }

            //slow down if moving through bush
            if(Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y) + gameObject.GetComponent<CircleCollider2D>().offset, 0.4f, LayerMask.GetMask("DeadEnemy")) != null){
                RB.linearDamping = 20f;
            } else{
                RB.linearDamping = 5f;
            }

            // target offset cooldown timer
            if (randCooldown > 0) {
                randCooldown -= Time.deltaTime;
            }

            //update animation
            anim.SetBool("moving", RB.linearVelocity.magnitude > 0.01f);
            anim.SetFloat("speed", RB.linearVelocity.magnitude);
        }
    }

    // hear a noise and update target if necessary
    public void recieveNoise(Vector2 newTarget, bool isNewTargetPriority)
    {
        // switch target if new is priority or current is invalid
        if ((!hasTarget || isNewTargetPriority) && !didTargetUpdate) {
            wanderCooldown = maxWanderCooldown; // ensure leader does not immediately make new wander target
            didTargetUpdate = true;
            preciseTarget = newTarget;
            int loopys = 0;
            do {
                loopys++;
                // randomize target offset
                if (randCooldown <= 0) {
                    randCooldown = Random.Range(3f, 18f);
                    randValues = new Vector2(Random.Range(-targetRandFactor, targetRandFactor), Random.Range(-targetRandFactor, targetRandFactor));
                }

                // if close to the randomization radius of target, disable rand to intercept
                if (Vector2.Distance(target, new Vector2(transform.position.x, transform.position.y)) < (targetRandFactor * 1.2f)) {
                    randValues = new Vector2();
                }

                target = new Vector2(newTarget.x + randValues.x, newTarget.y + randValues.y);
                targetDecayTimer = 0;
                noTargetUpdateTimer = 0;
            } while (loopys < 10 && ((target.x + transform.position.x) < mapEdgesXY[0] || (target.x + transform.position.x) > mapEdgesXY[1] || (target.y + transform.position.y) < mapEdgesXY[2] || (target.y + transform.position.y) > mapEdgesXY[3]));
            if (loopys > 9) {
                //Debug.Log("loopys problem 2 in enemycontroller; target:" + target);
                hasTarget = false;
            }
            
            // predictive targeting
            if (prevPreciseTarget.magnitude > 0 && isNewTargetPriority) {
                target += (((prevPreciseTarget - preciseTarget) / Time.deltaTime) * -predictiveValue * Mathf.Sqrt(Vector2.Distance(target, new Vector2(transform.position.x, transform.position.y))));
            }
            prevPreciseTarget = preciseTarget;

            targetPriority = isNewTargetPriority;
            hasTarget = true;
            overshootTimer = 0;
            if (isNewTargetPriority) { // echo to others if priority high
                echoNoise(newTarget, isNewTargetPriority, 0);
            }
        }
    }

    // communicate some recieved noises to other enemies nearby
    void echoNoise(Vector2 newTarget, bool isNewTargetPriority, float rangeOverride) {
        if (rangeOverride == 0) {
            rangeOverride = echoRadius;
        }
        // overlap circle to check for enemy tag
        enemiesFound = Physics2D.OverlapCircleAll(transform.position, rangeOverride, enemyMask); // the aforementioned circle
        for(int i = 0; i < enemiesFound.Length; i++) {
            enemiesFound[i].gameObject.GetComponent<EnemyController>().recieveNoise(newTarget, isNewTargetPriority);
        }
    }

    // recieve signal from colliding water to become static plant
    public void plantify () {
        if (!isPlant) {
            isPlant = true;
            // spawn static plant
            Instantiate(staticPlant, transform.position, transform.rotation);
            // delete self
            //GameObject.Destroy(gameObject);
            destroyTimer = 0.33333f;
        }
    }
    /*void OnCollisionEnter2D(Collision2D collision) {
        //Debug.Log("enemy touched " + collision.gameObject.transform.name);
        // forget target when hit barrier
        if (collision.gameObject.transform.name.Contains("Barrier")) {
            hasTarget = false;
        }
    }*/
}
