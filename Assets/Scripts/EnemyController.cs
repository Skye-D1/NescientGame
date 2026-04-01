using UnityEngine;

//Name: Skye Drury
//File: EnemyController.cs
//Purpose: Manage enemy movement, receiving and communicating noises, and other enemy behavior

public class EnemyController : MonoBehaviour
{
    public Vector2 target; // target movement location
    float moveSpeed = 300f; // speed of movement
    Vector3 movement = new Vector3(); // current movement direction
    public bool targetPriority; // whether the target is high priority (player noise)
    public bool hasTarget; // whether current target is valid
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
    float mapHeight = 54f;
    float mapWidth = 120f;
    float targetRandFactor = 3f;
    Rigidbody2D RB;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyMask = LayerMask.GetMask("Enemy");
        levelMask = LayerMask.GetMask("Default");
        target = new Vector2(transform.position.x, transform.position.y); // target own position on start
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        RB = gameObject.GetComponent<Rigidbody2D>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        
        // determine if leader
        if (Random.value > 0.4) { // todo: check proximity for other leaders
            isLeader = true;
            lineRenderer.startColor = new Color(1f,1f,0f,1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        didTargetUpdate = false; // allow one target update per frame
        
        // start moving past target for a time when reached
        if (targetPriority && Vector2.Distance(target, new Vector2(transform.position.x, transform.position.y)) < 1f && overshootTimer <= 0 && movement.magnitude > 0) {
            overshootTimer = 3f;
            hasTarget = false;
        } else if ((Vector2.Distance(target, new Vector2(transform.position.x, transform.position.y)) < 1f) || (!targetPriority && Vector2.Distance(target, new Vector2(transform.position.x, transform.position.y)) < 5f)) {
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
            movement = Vector3.Normalize(new Vector3(target.x, target.y, 0) - transform.position) * moveSpeed;
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
                } while (loopys < 100 && Mathf.Abs(newPoint.x + transform.position.x) > (mapWidth/2f) || Mathf.Abs(newPoint.y + transform.position.y) > (mapHeight/2f));
                if (loopys > 90) {
                    Debug.Log("loopys problem 1 in enemycontroller");
                }
                echoNoise(new Vector2(transform.position.x, transform.position.y) + newPoint, false, 10f);
                target = new Vector2(transform.position.x, transform.position.y) + newPoint;
                //Debug.Log("wander point broadcasted " + newPoint + " | " + transform.position);
                wanderCooldown = maxWanderCooldown + (maxWanderCooldown * (Random.value - 0.5f));
                didTargetUpdate = true;
            } else {
                wanderCooldown -= Time.deltaTime;
            }
        }
    }

    // hear a noise and update target if necessary
    public void recieveNoise(Vector2 newTarget, bool isNewTargetPriority)
    {
        // switch target if new is priority or current is invalid
        if ((!hasTarget || isNewTargetPriority) && !didTargetUpdate) {
            wanderCooldown = maxWanderCooldown; // ensure leader does not immediately make new wander target
            didTargetUpdate = true;
            int loopys = 0;
            do {
                loopys++;
                target = new Vector2(newTarget.x + Random.Range(-targetRandFactor, targetRandFactor), newTarget.y + Random.Range(-targetRandFactor, targetRandFactor));
            } while ((loopys < 100 && Mathf.Abs(target.x) > (mapWidth/2f) || Mathf.Abs(target.y) > (mapHeight/2f)));
            if (loopys > 90) {
                Debug.Log("loopys problem 2 in enemycontroller");
            }

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
            GameObject.Destroy(gameObject);
        }
    }
    void OnCollisionEnter2D(Collision2D collision) {
        // forget target when hit barrier
        if (collision.gameObject.transform.name.Contains("Barrier")) {
            hasTarget = false;
        // when enter bush go slower
        } else if (collision.gameObject.transform.name.Contains("enemyPlant")) {
            Debug.Log("enemy in bush");
            RB.linearDamping = 10f;
        }
    }

    void OnCollisionExit2D(Collision2D collision) {
        // when leave bush go faster
        if (collision.gameObject.transform.name.Contains("enemyPlant")) {
            Debug.Log("enemy out of bush");
            RB.linearDamping = 5f;
        }
    }
}
