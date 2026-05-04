/*
* Name: PlayerController.cs
* Author: Sam Johnson, Skye Drury
* Email: samuel.johnson, skye.drury
* Desc: Manage all player movement, input, and things affecting the player
*/

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    
    public GameObject projectile; // prefab for projectile
    public GameObject noiseCircle; // reference to circle for noise range debug
    Vector3 movement; // direction of movement
    Vector3 prevMovement; // movement last frame
    float moveSpeed = 500.0f; // how fast the player moves
    bool sprinting = false; // whether the player is sprinting this frame or not
    float sprintMult = 3.0f; // multiplier on how fast the player moves when sprinting
    float stamDrain = 30.0f; // how fast Stamina drains per second of sprinting
    float stamRegen = 10.0f; // how fast Stamina regenerates per second when not sprinting
    bool sneaking = false; // whether the player is sneaking
    float sneakMult = 0.35f; // how much slower the player moves while sneaking
    public float Stamina = 100.0f; // how much Stamina the player has
    public float Thirst = 100.0f; // how much Thirst the player has (100 = no Thirst, 0 = completely Thirsty)
    public float Health = 100.0f; // Health points
    public float Water = 100.0f; // how much Water is in the player's Water gun
    public float currentNoiseVolume = 0f; // per frame noise
    float prevNoiseVolume; // noise volume last frame
    float sneakNoiseVolume = 4f; // how loud the player is while sneaking
    float walkNoiseVolume = 7f; // how loud the player is when walking
    float sprintNoiseVolume = 25f; // how loud the player is while sprinting
    float waterGunNoiseVolume = 10f;
    float soundPulseDelay = 1f; // delay between noise pulse visualizations
    float soundPulseTimer = 0;
    LayerMask enemyMask;
    int selectedInvSlot = 0;
    float[,] inventory = new float[3,2];
    public GameObject[] itemKey;
    float hitCooldown = 0;
    public Sprite[] bottleSprites;
    public bool dying;
    public bool preventDie; // debug probably
    public bool debugLasers; // debuggin emeny
    GameObject hudWaterGun;
    SpriteRenderer selfRenderer;
    bool useMouseForLook;
    public bool isPaused = false;
    Vector3 lastCameraChange;
    //float cameraLagRatio = 100f;
    Vignette vignette;
    TextMeshPro fpsCounter;
    TextMeshPro scoreDisplay;
    TextMeshPro highScoreDisplay;
    
    public TextMeshPro deltaScoreDisplay; // displays changes in score to player
    float fpsUpdateTimer;
    public GameObject hedgeCut;
    Animator anim;
    float forceSoundPulseCooldown = 0;
    enemySpawning spawner;
    SpriteRenderer dmgFrameOverlay;
    float dmgOpacity;
    public float gameScore;
    public float deltaScoreOpacity;
    ScoreData scoreData;
    float deathTime;
    public Vector2 tutorialForcedTarget;
    public bool isTutorialTime = true;
    bool drumTrack = false;
    bool onWood = false;
    public GameObject waterTankOverlay;
    public float center;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //fetches reference and assigns necessary variables
        anim = gameObject.GetComponent<Animator>();
        enemyMask = LayerMask.GetMask("Enemy"); // set layer mask
        dmgFrameOverlay = GameObject.Find("damageFrameOverlay").GetComponent<SpriteRenderer>();
        //dmgFrameOverlay.enabled = false;
        dmgFrameOverlay.color = new Color(1f,0f,0f,(0f));
        fpsCounter = GameObject.Find("fpsCounter").GetComponentInChildren<TextMeshPro>();
        scoreDisplay = GameObject.Find("scoreDisplay").GetComponentInChildren<TextMeshPro>();
        deltaScoreDisplay = GameObject.Find("deltaScoreDisplay").GetComponentInChildren<TextMeshPro>();
        highScoreDisplay = GameObject.Find("highScoreDisplay").GetComponentInChildren<TextMeshPro>();
        
        spawner = GameObject.Find("spawner").GetComponentInChildren<enemySpawning>();
        hudWaterGun = GameObject.Find("HUDWaterGun");
        selfRenderer = gameObject.GetComponent<SpriteRenderer>();

        scoreData = GameObject.Find("dataKeeper").GetComponent<ScoreData>();
        DontDestroyOnLoad(scoreData.gameObject);

        // Disable VSync to use target frameRate
        //QualitySettings.vSyncCount = 1;

        // Set target frame rate to 60 FPS
        Application.targetFrameRate = 60;

        //linerenderer
        /*
        LineRenderer noiseCircle = GameObject.Find("debug_noise_range").GetComponent<LineRenderer>();
        Vector3[] points = new Vector3[360];
        for(int i = 0; i < 360; i++){
            float r = Mathf.Sin(Mathf.Deg2Rad*i*10) + 1f;
            points[i] = new Vector3(r*Mathf.Sin(Mathf.Deg2Rad*i*10),r*Mathf.Cos(Mathf.Deg2Rad*i*10),0);
        }
        noiseCircle.SetPositions(points);*/
    }

    // Update is called once per frame
    void Update()
    {
        if(!isTutorialTime && !drumTrack){
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayClip(1, false);
            drumTrack = true;
        }
        // decrease damage overlay at start of every frame
        if (dmgOpacity > 0) {
            dmgOpacity -= Time.deltaTime * 1f;
        }

        highScoreDisplay.text = "high score: " + Mathf.Round(scoreData.highScore);

        // decrease score change overlay at start of every frame
        if (deltaScoreOpacity > 0) {
            deltaScoreOpacity -= Time.deltaTime * 0.4f;
        }
        deltaScoreDisplay.color = new Color(deltaScoreDisplay.color.r,deltaScoreDisplay.color.g,deltaScoreDisplay.color.b,(deltaScoreOpacity));

        fpsUpdateTimer += Time.deltaTime;
        if (fpsUpdateTimer > 0.5f) {
            fpsCounter.text = "fps:" + Mathf.Round(1f / Time.deltaTime);
            fpsUpdateTimer = 0;
        }
        forceSoundPulseCooldown -= Time.deltaTime;

        if (vignette == null) {
            GameObject.Find("Volume").GetComponent<Volume>().profile.TryGet(out vignette);
        }
        
        if((Health <= 0 || Thirst <= 0) && !preventDie && (!(vignette.intensity.value < 1f) && !(vignette.smoothness.value < 1f))){
            isPaused = true;
            Time.timeScale = 0.01f;
            // update high score
            if (gameScore > scoreData.highScore) {
                scoreData.highScore = gameScore;
            }
            // record death time to stay here for a bit
            if (deathTime == 0) {
                deathTime = Time.realtimeSinceStartup;
            }
            if (Time.realtimeSinceStartup - deathTime > 5f) {
                // restart at main menu
                SceneManager.LoadScene("MainScene");
            }
        }
        if (tutorialForcedTarget != new Vector2()) {
            // move to tutorial spot
            movement = Vector3.Normalize(new Vector3(tutorialForcedTarget.x, tutorialForcedTarget.y, 0) - transform.position) * moveSpeed;
            gameObject.GetComponent<Rigidbody2D>().AddForce(movement * Time.deltaTime);
            if (Vector2.Distance(tutorialForcedTarget, new Vector2(transform.position.x, transform.position.y)) < 0.5f) {
                tutorialForcedTarget = new Vector2();
            }
            anim.SetFloat("walkMult", 1f);
            anim.SetFloat("sprintMult", 3f);
            anim.SetFloat("sneakMult", 0.35f);

        } else if(!isPaused){
            if (!isTutorialTime) {
                // update score and display
                gameScore += Time.deltaTime;
            }
            scoreDisplay.text = "score: " + Mathf.Round(gameScore);
            
            //death
            if((Health <= 0 || Thirst <= 0) && !preventDie){
                dying = true;
                //Debug.Log("dying: " + dying);
                if(vignette.intensity.value < 1f){
                    vignette.intensity.value += Time.deltaTime;
                }else if(vignette.smoothness.value < 1f){
                    vignette.smoothness.value += Time.deltaTime;
                }
            }

            //defining how the player should move this frame
            movement = new Vector3();
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            // enable enemy spawning when player leaves tutorial house
            if (transform.position.x < 8f || transform.position.x > 25f || transform.position.y < 9f || transform.position.y > 16f) { 
                spawner.doSpawning = true;
                isTutorialTime = false;
            }

            //look direction
            if (prevMovement != movement) { // if changed move input, loop at move direction
                useMouseForLook = false;
            }
            if (movement.x > 0) {
                selfRenderer.flipX = true;
            } else if (movement.x < 0) {
                selfRenderer.flipX = false;
            } else { // if not movin, look at mouse
                useMouseForLook = true;
            }
            if (Input.mousePositionDelta.magnitude > 1) { // if mouse movin, look at mouse
                useMouseForLook = true;
            }
            if (useMouseForLook) {
                //flip towards mouse
                if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > transform.position.x) {
                    selfRenderer.flipX = true;
                } else if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x) {
                    selfRenderer.flipX = false;
                }

                //if mouse look direction is different from regular look direction, reverse animation speed
                if((Camera.main.ScreenToWorldPoint(Input.mousePosition).x > transform.position.x) != (movement.x > 0)){
                    anim.SetFloat("walkMult", -1f);
                    anim.SetFloat("sprintMult", -3f);
                    anim.SetFloat("sneakMult", -0.35f);
                } else{
                    anim.SetFloat("walkMult", 1f);
                    anim.SetFloat("sprintMult", 3f);
                    anim.SetFloat("sneakMult", 0.35f);
                }
            } else{
                anim.SetFloat("walkMult", 1f);
                anim.SetFloat("sprintMult", 3f);
                anim.SetFloat("sneakMult", 0.35f);
            }
            prevMovement = movement;
            
            movement = Vector3.Normalize(movement)*moveSpeed; // normalize and set speed of movement in direction

            //is the player sprinting or sneaking? Stamina regeneration if they aren't sprinting
            if((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && movement != new Vector3()){
                sprinting = true;
            } else{
                sprinting = false;
                if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftControl)){
                    sneaking = true;
                } else{
                    sneaking = false;
                }
                if(Stamina + stamRegen * Time.deltaTime < 100){
                    Stamina += stamRegen * Time.deltaTime;
                } else{
                    Stamina = 100.0f;
                }
                
            }

            //Stamina drain and using movement
            if(sprinting && Stamina - stamDrain * Time.deltaTime > 0){
                gameObject.GetComponent<Rigidbody2D>().AddForce(movement*sprintMult*Time.deltaTime);
                Stamina = Stamina - stamDrain * Time.deltaTime;
            } else if(sneaking){
                gameObject.GetComponent<Rigidbody2D>().AddForce(movement*sneakMult*Time.deltaTime);
            } else{
                gameObject.GetComponent<Rigidbody2D>().AddForce(movement*Time.deltaTime);
            }

            //Thirst drain based on Stamina
            /*if(Thirst - Time.deltaTime * ((100 - Stamina)/50 + 0.1f) > 0){
                Thirst -= Time.deltaTime * ((100 - Stamina)/50 + 0.1f);
            } else{
                Thirst = 0;
            }*/

            //looking at water gun logic
            Vector3 cameraPos = GameObject.Find("Main Camera").transform.position;
            //going up
            /*if(Input.GetKey(KeyCode.F)){
                if(hudWaterGun.transform.position.y - cameraPos.y != 0){
                    hudWaterGun.transform.position += new Vector3(0,Time.deltaTime*14f,0);
                    if(hudWaterGun.transform.position.y - cameraPos.y > 0){
                        //Debug.Log("HUD view up!!!");
                        hudWaterGun.transform.position = cameraPos + new Vector3(0,0,10f);
                    }
                }
            }
            //going down
            else if(!Input.GetKey(KeyCode.F) && hudWaterGun.transform.position.y - cameraPos.y != -9.75f){
                hudWaterGun.transform.position -= new Vector3(0,Time.deltaTime*9.75f,0);
                if(hudWaterGun.transform.position.y - cameraPos.y < -9.75){
                    //Debug.Log("HUD view down!!!");
                    hudWaterGun.transform.position = cameraPos + new Vector3(0,-9.75f,100);
                }
            }*/

            //inventory
            GameObject.Find("invSlot" + selectedInvSlot).GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,0.6f);
            int pastInvSlot = selectedInvSlot;

            //scroll to swap slots
            selectedInvSlot += (int) (Input.GetAxisRaw("Mouse ScrollWheel") * -10f);
            while(selectedInvSlot >= 3){
                selectedInvSlot -= 3;
            } while(selectedInvSlot < 0){
                selectedInvSlot += 3;
            }
            
            //numbers to swap slots
            if(Input.GetKeyDown(KeyCode.Alpha1)){
                selectedInvSlot = 0;
            } else if(Input.GetKeyDown(KeyCode.Alpha2)){
                selectedInvSlot = 1;
            } else if(Input.GetKeyDown(KeyCode.Alpha3)){
                selectedInvSlot = 2;
            }

            //click to swap slots
            bool isShoot = false;
            if(Input.GetButtonDown("Fire1")){
                isShoot = true;
                for(int i = 0; i < 3; i++){
                    GameObject slot = GameObject.Find("invSlot" + i);
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if(Mathf.Abs(mousePos.x-slot.transform.position.x) < 1f && Mathf.Abs(mousePos.y-slot.transform.position.y) < 1f){
                        isShoot = false;
                        selectedInvSlot = i;
                    }
                }
            }

            if(selectedInvSlot != pastInvSlot){
                GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayClip(2 + 3*(int)inventory[selectedInvSlot,0], false);
            }

            GameObject.Find("invSlot" + selectedInvSlot).GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,1f);

            //Water Gun shoot
            Vector3 projSourcePoint = new Vector3(transform.position.x + (selfRenderer.flipX ? 0.8f : -0.8f), transform.position.y + 0.3f, transform.position.z);
            if(isShoot && Water >= 4f && Physics2D.Raycast(transform.position, new Vector2((selfRenderer.flipX ? 0.8f : -0.8f), 0.3f), 1f, LayerMask.GetMask("ProjectileBlocker")).collider == null){
                GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayClip(21, false);
                Water -= 4f;
                useMouseForLook = true;
                Vector3 dir = Vector3.Normalize(Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0,0,10) - projSourcePoint);
                currentNoiseVolume = waterGunNoiseVolume;
                forceSoundPulseVisual();

                //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0,0,10) -transform.position);

                for(int i = 0; i < 10; i++){
                    //randomize angle of each projectile
                    float radians = Random.Range(-3f,3f) * Mathf.Deg2Rad;
                    float sin = Mathf.Sin(radians);
                    float cos = Mathf.Cos(radians);
                    float newX = dir.x * cos - dir.y * sin;
                    float newY = dir.x * sin + dir.y * cos;
                    dir.x = newX; dir.y = newY;

                    //default force of each projectile
                    float force = 1000f;

                    //randomize force slightly
                    float perc = Random.Range(-0.15f, 0.15f);
                    force = force * (1 + perc);

                    //make and add force to projectile
                    GameObject proj = Instantiate(projectile, projSourcePoint, new Quaternion());
                    proj.GetComponent<Rigidbody2D>().AddForce(dir * force);
                }
            }
            
            //pickup
            if(Input.GetKeyDown(KeyCode.E)){
                Collider2D[] itemsFound = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y) + gameObject.GetComponent<Collider2D>().offset, 2.0f, LayerMask.GetMask("Item"));
                if(itemsFound.Length > 0){
                    //find closest item
                    float itemDist = 3f;
                    GameObject closestItem = null;
                    foreach(Collider2D item in itemsFound){
                        if(Vector2.Distance(new Vector2(transform.position.x, transform.position.y) + gameObject.GetComponent<Collider2D>().offset, new Vector2(item.gameObject.transform.position.x, item.gameObject.transform.position.y)) < itemDist){
                            itemDist = Vector2.Distance(new Vector2(transform.position.x, transform.position.y) + gameObject.GetComponent<Collider2D>().offset, new Vector2(item.gameObject.transform.position.x, item.gameObject.transform.position.y));
                            closestItem = item.gameObject;
                        }
                    }
                    
                    //put item in free slot if available and then update sprite if it is a water bottle
                    int putSlot = 4;
                    if(inventory[selectedInvSlot, 0] == 0){
                        putSlot = selectedInvSlot;
                    } else{
                        if(inventory[0, 0] == 0){
                            putSlot = 0;
                        } else if(inventory[1, 0] == 0){
                            putSlot = 1;
                        } else if(inventory[2, 0] == 0){
                            putSlot = 2;
                        }
                    }
                    if(putSlot != 4){
                        inventory[putSlot, 0] = closestItem.GetComponent<Item>().itemID;
                        inventory[putSlot, 1] = closestItem.GetComponent<Item>().power;
                        GameObject.Destroy(closestItem);

                        Debug.Log("pickup sound");
                        GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayClip(3*closestItem.GetComponent<Item>().itemID, false);
                    }
                }
            }

            //drop
            if(Input.GetKeyDown(KeyCode.Q)){
                DropItem();
                inventory[selectedInvSlot, 0] = 0;
                inventory[selectedInvSlot, 1] = 0;
            }

            //use item
            if(Input.GetButtonDown("Fire2")){
                if(inventory[selectedInvSlot, 0] != 0){
                    if(inventory[selectedInvSlot, 0] == 1 && Water != 100f){
                        //Water Bottle
                        GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayClip(1 + 3*(int)inventory[selectedInvSlot,0], false);
                        if(100f-Water >= inventory[selectedInvSlot, 1]){
                            Water += inventory[selectedInvSlot, 1];
                            inventory[selectedInvSlot, 0] = 0;
                            inventory[selectedInvSlot, 1] = 0;
                        } else if(100f-Water < inventory[selectedInvSlot, 1]){
                            inventory[selectedInvSlot, 1] -= 100f-Water;
                            Water = 100f;
                        }

                    } else if(inventory[selectedInvSlot, 0] == 2 && Health != 100.0f){
                        //Health Item
                        GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayClip(1 + 3*(int)inventory[selectedInvSlot,0], false);
                        Health+=25;
                        if(Health > 100f){
                            Health = 100f;
                        }
                        inventory[selectedInvSlot, 0] = 0;
                        inventory[selectedInvSlot, 1] = 0;

                    } else if(inventory[selectedInvSlot, 0] == 3){
                        //Hedge Cutter
                        GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayClip(1 + 3*(int)inventory[selectedInvSlot,0], false);
                        Collider2D[] deadEnemies = Physics2D.OverlapCircleAll((new Vector2(transform.position.x, transform.position.y) + gameObject.GetComponent<Collider2D>().offset) + (new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y) - new Vector2(transform.position.x, transform.position.y)).normalized * 2f, 1.5f, LayerMask.GetMask("DeadEnemy"));
                        Vector2 mouseDirFromWorld = (new Vector2(transform.position.x, transform.position.y) + gameObject.GetComponent<Collider2D>().offset) + (new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y) - new Vector2(transform.position.x, transform.position.y)).normalized;
                        GameObject cut = GameObject.Instantiate(hedgeCut, (new Vector2(transform.position.x, transform.position.y) + gameObject.GetComponent<Collider2D>().offset) + (new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y) - new Vector2(transform.position.x, transform.position.y)).normalized * 0f, Quaternion.identity);
                        cut.transform.LookAt(new Vector3(mouseDirFromWorld.x, mouseDirFromWorld.y, 0));
                        cut.transform.Rotate(0,90,90);

                        foreach(Collider2D bush in deadEnemies){
                            GameObject.Destroy(bush.gameObject);
                        }
                        // destroy item
                        inventory[selectedInvSlot, 0] = 0;
                        inventory[selectedInvSlot, 1] = 0;
                    }
                }
            }

            

            UpdateInventory();

            foreach(Item item in FindObjectsByType<Item>(0)){
                if(item.itemID == 1){
                    if(item.power > 75f){
                        item.gameObject.GetComponent<SpriteRenderer>().sprite = bottleSprites[3];
                    } else if(item.power > 50f){
                        item.gameObject.GetComponent<SpriteRenderer>().sprite = bottleSprites[2];
                    } else if(item.power > 25){
                        item.gameObject.GetComponent<SpriteRenderer>().sprite = bottleSprites[1];
                    }else{
                        item.gameObject.GetComponent<SpriteRenderer>().sprite = bottleSprites[0];
                    }
                }
            }

            // alert enemies with noise - Skye
            if (sprinting && movement.magnitude != 0) {
                currentNoiseVolume = sprintNoiseVolume;
                if (prevNoiseVolume < sprintNoiseVolume) {
                    forceSoundPulseVisual();
                }
            } else if (movement.magnitude != 0 && !sneaking) {
                currentNoiseVolume = walkNoiseVolume;
                if (prevNoiseVolume < walkNoiseVolume) {
                    forceSoundPulseVisual();
                }
            } else if (movement.magnitude != 0 && sneaking) {
                currentNoiseVolume = sneakNoiseVolume;
                if (prevNoiseVolume < sneakNoiseVolume) {
                    forceSoundPulseVisual();
                }
            }

            prevNoiseVolume = currentNoiseVolume;

            // overlap circle to check for enemy tag - Skye
            Collider2D[] enemiesFound = Physics2D.OverlapCircleAll(transform.position, currentNoiseVolume, enemyMask);
            for(int i = 0; i < enemiesFound.Length; i++) {
                enemiesFound[i].gameObject.GetComponent<EnemyController>().recieveNoise(new Vector2(transform.position.x, transform.position.y), true);
            }
            if (soundPulseTimer < 0) { // sound pulse happen on timed repeat
                soundPulseTimer = soundPulseDelay;
                GameObject newNoiseCircle = Instantiate(noiseCircle, transform.position, new Quaternion());
                newNoiseCircle.GetComponent<noiseCircleController>().noiseRange = currentNoiseVolume;
            } else {
                soundPulseTimer -= Time.deltaTime;
            }

            //cooldown on how often enemies can hit player
            if(hitCooldown > 0){
                hitCooldown -= Time.deltaTime;
            }

            //does enemy hit player?
            if(hitCooldown <= 0){
                Collider2D[] EnemyColliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y) + gameObject.GetComponent<CircleCollider2D>().offset, 0.5f, LayerMask.GetMask("Enemy"));
                Collider2D[] DeadEnemyColliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y) + gameObject.GetComponent<CircleCollider2D>().offset, 0.5f, LayerMask.GetMask("DeadEnemy"));
                if(EnemyColliders.Length > 0){
                    bool hit = false;
                    foreach(Collider2D col in EnemyColliders){
                        if(col.GetType() == typeof(CircleCollider2D)){
                            hit = true;
                            break;
                        }
                    }
                    if(hit){
                        GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayClip(19, false);
                        Health -= 25f;
                        hitCooldown = 1f;
                        dmgOpacity = 0.3f;
                    }
                }
                else if(DeadEnemyColliders.Length > 0){
                    bool hit = false;
                    foreach(Collider2D col in DeadEnemyColliders){
                        if(col.GetType() == typeof(CircleCollider2D)){
                            hit = true;
                            break;
                        }
                    }
                    if(hit){
                        Health -= 15f;
                        hitCooldown = 1f;
                        dmgOpacity = 0.3f;
                    }
                }
            }

            // update water gauge needle position
            float currentZ = hudWaterGun.transform.GetChild(0).transform.eulerAngles.z;
            float targetZ = Mathf.Lerp(140f, -140f, (Water/100f));
            if (currentZ > 180) {
                currentZ -= 360;
            }
            hudWaterGun.transform.GetChild(0).transform.Rotate(0, 0, ((Mathf.Abs(targetZ - currentZ) > 2) ? Mathf.Sign(targetZ - currentZ) * 3f : (targetZ - currentZ) * 0.03f)); // .Rotate uses euler angles
            
            // update water tank water position
            if (waterTankOverlay != null) {
                
                waterTankOverlay.transform.localScale = new Vector3(1, (Water)/100f * 1f, 1f);
                waterTankOverlay.transform.localPosition = new Vector3(waterTankOverlay.transform.localPosition.x, center - ((100f-Water) / 80f), 0);
                //Debug.Log("water y: " + (center - ((100f-Water) / 200f)) + " center: " + center);
            }
            
            currentNoiseVolume = 2f; // base noise volume for next frame

            //enemy chase drums
            /*Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y) + gameObject.GetComponent<Collider2D>().offset, 14f, LayerMask.GetMask("Enemy"));
            AudioSource drums = gameObject.GetComponent<AudioSource>();
            if(enemiesInRange.Length > 0){
                if(enemiesInRange.Length == 1){
                    float dist = Vector3.Distance(enemiesInRange[0].transform.position, transform.position);
                    //Debug.Log(dist);
                    if(dist <= 2f){
                        drums.volume = 1f;
                    } else{
                        drums.volume = (12f-dist)/12f;
                    }
                } else{
                    float dist = 14f;
                    for(int i = 0; i < enemiesInRange.Length; i++){
                        if(Vector3.Distance(enemiesInRange[i].transform.position, transform.position) < dist){
                            dist = Vector3.Distance(enemiesInRange[i].transform.position, transform.position);
                        }
                    }
                    //Debug.Log(dist);
                    if(dist <= 2f){
                        drums.volume = 1f;
                    } else{
                        drums.volume = (12f-dist)/12f;
                    }
                }
            } else{
                drums.volume = 0f;
            }*/

            //camera lag behind
            /*
            Vector3 newPos = new Vector3(transform.position.x + -0.25f * (gameObject.GetComponent<Rigidbody2D>().linearVelocity.x * cameraLagRatio * Time.deltaTime + lastCameraChange.x * (1-cameraLagRatio * Time.deltaTime)), transform.position.y + -0.25f * (gameObject.GetComponent<Rigidbody2D>().linearVelocity.y * cameraLagRatio * Time.deltaTime + lastCameraChange.y * (1-cameraLagRatio * Time.deltaTime)), -10f);
            lastCameraChange = GameObject.Find("Main Camera").transform.position - newPos;
            GameObject.Find("Main Camera").transform.position = newPos;
            */
            Vector2 camera = new Vector2(GameObject.Find("Main Camera").transform.position.x, GameObject.Find("Main Camera").transform.position.y);
            Vector2 direction = (new Vector2(transform.position.x, transform.position.y) - camera);
            GameObject.Find("Main Camera").transform.position += new Vector3(direction.x / 0.5f * Time.deltaTime, direction.y / 0.5f * Time.deltaTime, 0);
            
            //update animator
            anim.SetBool("Moving", !(movement == new Vector3()));
            if(Stamina > 5 && sprinting){
                anim.SetBool("Sprinting", true);
            } else{
                anim.SetBool("Sprinting", false);
            }
            
            anim.SetBool("Sneaking", sneaking);
            dmgFrameOverlay.color = new Color(1f,0f,0f,(dmgOpacity));
        }
    }

    /*
    Name: UpdateInventory
    Desc: refreshes the in-scene aspects of the inventory
    */
    void UpdateInventory(){
        for(int i = 0; i < 3; i++){
            GameObject slot = GameObject.Find("invSlot" + i);
            if((slot.transform.childCount != 0 && inventory[i,0] == 0) || (slot.transform.childCount != 0 && slot.transform.GetChild(0).gameObject.GetComponent<Item>().itemID != inventory[i,0])){
                GameObject.Destroy(slot.transform.GetChild(0).gameObject);
            }
            if(slot.transform.childCount != 0 && inventory[i,0] != 0){
                slot.transform.GetChild(0).gameObject.GetComponent<Item>().power = inventory[i,1];
            }
            if(inventory[i,0]!=0 && slot.transform.childCount == 0){
                GameObject item = Instantiate(itemKey[(int)inventory[i,0]], GameObject.Find("invSlot" + i).transform.position, new Quaternion(), GameObject.Find("invSlot" + i).transform);
                item.GetComponent<Item>().itemID = (int)inventory[i,0];
                item.GetComponent<Item>().power = inventory[i,1];
                item.transform.localScale = new Vector3(30f, 30f, 1f);
                item.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
            }
        }
    }

    /*
    Name: DropItem
    Desc: Drops the item in the currently selected inventory slot from the inventory into the scene at the feet of the player
    */
    void DropItem(){
        if(inventory[selectedInvSlot,0] != 0){
            GameObject item = Instantiate(itemKey[(int)inventory[selectedInvSlot,0]], new Vector2(transform.position.x, transform.position.y) + gameObject.GetComponent<Collider2D>().offset, new Quaternion());
            item.GetComponent<Item>().power = inventory[selectedInvSlot, 1];
        }
        
    }

    /*
    Name: forceSoundPulseVisual
    Desc: forces the sound visual to pulse if the cooldown isn't active.
    */
    void forceSoundPulseVisual() {
        if (forceSoundPulseCooldown <= 0) {
            soundPulseTimer = -1;
            forceSoundPulseCooldown = 0.5f;
        }
    }

    void playStepSound(){
        if(!sprinting && !onWood){
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayClip(12, false);
        } else if(sprinting && !onWood){
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayClip(13, false);
        } else if(!sprinting && onWood){
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayClip(14, false);
        } else if(sprinting && onWood){
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayClip(15, false);
        } else{
            Debug.Log("step sound error");
        }
    }

    void OnTriggerEnter2D(Collider2D collider){
        if(collider.CompareTag("Bush")){
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayClip(16, false);
        } else if(collider.CompareTag("onWood")){
            Debug.Log("ON WOOD");
            onWood = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider){
        if(collider.CompareTag("onWood")){
            Debug.Log("NOT ON WOOD");
            onWood = false;
        }
    }
}
