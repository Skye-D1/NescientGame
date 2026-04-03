using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//Name: Sam Johnson, Skye Drury
//File: PlayerController.cs
//Purpose: Manage all player movement, input, and things affecting the player

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
    bool dying;
    public bool preventDie; // debug probably
    public bool debugLasers; // debuggin emeny
    GameObject hudWaterGun;
    SpriteRenderer selfRenderer;
    bool useMouseForLook;
    public bool isPaused = false;
    Vector3 lastCameraChange;
    float cameraLagRatio = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyMask = LayerMask.GetMask("Enemy"); // set layer mask

        hudWaterGun = GameObject.Find("HUDWaterGun");
        selfRenderer = gameObject.GetComponent<SpriteRenderer>();

        // Disable VSync to use target frameRate
        QualitySettings.vSyncCount = 1;

        // Set target frame rate to 120 FPS
        Application.targetFrameRate = 120;

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
        if(!isPaused){
            Vignette vignette = GameObject.Find("Main Camera").GetComponent<CameraEffects>().vignette;
            if((Health <= 0 || Thirst <= 0) && !preventDie){
                dying = true;
                //Debug.Log("dying: " + dying);
                if(vignette.intensity.value < 1f){
                    vignette.intensity.value += Time.deltaTime;
                }else if(vignette.smoothness.value < 1f){
                    vignette.smoothness.value += Time.deltaTime;
                } else{
                    isPaused = true;
                    Time.timeScale = 0;
                }
            }

            //defining how the player should move this frame
            movement = new Vector3();
            movement.x = Input.GetAxisRaw("Horizontal");
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
                if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > transform.position.x) {
                    selfRenderer.flipX = true;
                } else if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x) {
                    selfRenderer.flipX = false;
                }
            }
            prevMovement = movement;

            movement.y = Input.GetAxisRaw("Vertical");
            movement = Vector3.Normalize(movement)*moveSpeed; // normalize and set speed of movement in direction

            //is the player sprinting or sneaking? Stamina regeneration if they aren't sprinting
            if(Input.GetKey(KeyCode.LeftShift) && movement != new Vector3()){
                sprinting = true;
            } else{
                sprinting = false;
                if(Input.GetKey(KeyCode.LeftControl)){
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
            if(Thirst - Time.deltaTime * ((100 - Stamina)/50 + 0.1f) > 0){
                Thirst -= Time.deltaTime * ((100 - Stamina)/50 + 0.1f);
            } else{
                Thirst = 0;
            }

            //Water Gun shoot
            if(Input.GetButtonDown("Fire1") && Water >= 10.0f){
                Water -= 10;
                useMouseForLook = true;
                Vector3 dir = Vector3.Normalize(Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0,0,10) - (transform.position + new Vector3(gameObject.GetComponent<Collider2D>().offset.x, gameObject.GetComponent<Collider2D>().offset.y, 0)));
                currentNoiseVolume = waterGunNoiseVolume;
                soundPulseTimer = -1; // force sound pulse visual

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
                    GameObject proj = Instantiate(projectile, transform.position, new Quaternion());
                    proj.GetComponent<Rigidbody2D>().AddForce(dir * force);
                }
            }

            //looking at water gun logic
            Vector3 cameraPos = GameObject.Find("Main Camera").transform.position;
            //going up
            if(Input.GetKey(KeyCode.F)){
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
            }

            //inventory
            GameObject.Find("invSlot" + selectedInvSlot).GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,0.6f);
            selectedInvSlot += (int) (Input.GetAxisRaw("Mouse ScrollWheel") * -10f);
            while(selectedInvSlot >= 3){
                selectedInvSlot -= 3;
            } while(selectedInvSlot < 0){
                selectedInvSlot += 3;
            }
            GameObject.Find("invSlot" + selectedInvSlot).GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,1f);
            
            //pickup
            bool playPickupSound = true;
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
                    
                    //put item in free slot if available
                    if(inventory[selectedInvSlot, 0] == 0){
                        inventory[selectedInvSlot, 0] = closestItem.GetComponent<Item>().itemID;
                        inventory[selectedInvSlot, 1] = closestItem.GetComponent<Item>().power;
                        GameObject.Destroy(closestItem);
                    } else{
                        if(inventory[0, 0] == 0){
                            inventory[0, 0] = closestItem.GetComponent<Item>().itemID;
                            inventory[0, 1] = closestItem.GetComponent<Item>().power;
                            GameObject.Destroy(closestItem);
                        } else if(inventory[1, 0] == 0){
                            inventory[1, 0] = closestItem.GetComponent<Item>().itemID;
                            inventory[1, 1] = closestItem.GetComponent<Item>().power;
                            GameObject.Destroy(closestItem);
                        } else if(inventory[2, 0] == 0){
                            inventory[2, 0] = closestItem.GetComponent<Item>().itemID;
                            inventory[2, 1] = closestItem.GetComponent<Item>().power;
                            GameObject.Destroy(closestItem);
                        } else{
                            playPickupSound = false;
                        }
                    }

                    if(playPickupSound){
                        GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayClip(2*closestItem.GetComponent<Item>().itemID, false);
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
                    
                    GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayClip(1 + 2*(int)inventory[0,0], false);
                    
                    if(inventory[selectedInvSlot, 0] == 1){
                        //Water Bottle
                        if(Input.GetKey(KeyCode.F)){
                            if(100f-Water >= inventory[selectedInvSlot, 1]){
                                Water += inventory[selectedInvSlot, 1];
                                inventory[selectedInvSlot, 0] = 0;
                                inventory[selectedInvSlot, 1] = 0;
                            } else if(100f-Water < inventory[selectedInvSlot, 1]){
                                inventory[selectedInvSlot, 1] -= 100f-Water;
                                Water = 100f;
                            }
                        }else{
                            if(100f-Thirst >= inventory[selectedInvSlot, 1]){
                                Thirst += inventory[selectedInvSlot, 1];
                                inventory[selectedInvSlot, 0] = 0;
                                inventory[selectedInvSlot, 1] = 0;
                            } else if(100f-Thirst < inventory[selectedInvSlot, 1]){
                                inventory[selectedInvSlot, 1] -= 100f-Thirst;
                                Thirst = 100f;
                            }
                        }
                        

                        //change sprite of water bottle based on water level
                        if(inventory[selectedInvSlot, 1] > 75f){
                            GameObject.Find("invSlot" + selectedInvSlot).transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = bottleSprites[3];
                        } else if(inventory[selectedInvSlot, 1] > 50f){
                            GameObject.Find("invSlot" + selectedInvSlot).transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = bottleSprites[2];
                        } else if(inventory[selectedInvSlot, 1] > 25){
                            GameObject.Find("invSlot" + selectedInvSlot).transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = bottleSprites[1];
                        }else{
                            GameObject.Find("invSlot" + selectedInvSlot).transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = bottleSprites[0];
                        }

                    } else if(inventory[selectedInvSlot, 0] == 2){
                        //Health Item
                        if(Health != 100.0f){
                            Health+=25;
                            if(Health > 100f){
                                Health = 100f;
                            }
                            inventory[selectedInvSlot, 0] = 0;
                            inventory[selectedInvSlot, 1] = 0;
                        }

                    } else if(inventory[selectedInvSlot, 0] == 3){
                        //Hedge Clippers
                        Collider2D[] deadEnemies = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y) + gameObject.GetComponent<Collider2D>().offset + (new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y) - new Vector2(transform.position.x, transform.position.y)).normalized, 2f, LayerMask.GetMask("DeadEnemy"));
                        //GameObject circle = GameObject.Instantiate(debugCircle, new Vector2(transform.position.x, transform.position.y) + gameObject.GetComponent<Collider2D>().offset + (new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y) - new Vector2(transform.position.x, transform.position.y)).normalized, new Quaternion());
                        //circle.transform.localScale = new Vector3(2f,2f, 1f);
                        

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

            // alert enemies with noise - Skye
            if (sprinting && movement.magnitude != 0) {
                currentNoiseVolume = sprintNoiseVolume;
                if (prevNoiseVolume < sprintNoiseVolume) {
                    soundPulseTimer = -1; // force sound pulse visual
                }
            } else if (movement.magnitude != 0 && !sneaking) {
                currentNoiseVolume = walkNoiseVolume;
                if (prevNoiseVolume < walkNoiseVolume) {
                    soundPulseTimer = -1; // force sound pulse visual
                }
            } else if (movement.magnitude != 0 && sneaking) {
                currentNoiseVolume = sneakNoiseVolume;
                if (prevNoiseVolume < sneakNoiseVolume) {
                    soundPulseTimer = -1; // force sound pulse visual
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
                if(Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y) + gameObject.GetComponent<CircleCollider2D>().offset, 0.5f, LayerMask.GetMask("Enemy")) != null && (Health - 5) >= 0){
                    Health -= 30;
                    hitCooldown = 0.5f;
                }
                else if(Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y) + gameObject.GetComponent<CircleCollider2D>().offset, 0.5f, LayerMask.GetMask("DeadEnemy")) != null && (Health - 5) >= 0){
                    Health -= 15;
                    hitCooldown = 0.5f;
                }
            }

            // update water gauge needle position
            float currentZ = hudWaterGun.transform.GetChild(0).transform.eulerAngles.z;
            float targetZ = Mathf.Lerp(140f, -140f, (Water/100f));
            if (currentZ > 180) {
                currentZ -= 360;
            }
            hudWaterGun.transform.GetChild(0).transform.Rotate(0, 0, Mathf.Sign(targetZ - currentZ) * Time.deltaTime * 300f); // .Rotate uses euler angles
            
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
        }
    }

    void UpdateInventory(){
        for(int i = 0; i < 3; i++){
            GameObject slot = GameObject.Find("invSlot" + i);
            if((slot.transform.childCount != 0 && inventory[i,0] == 0) || (slot.transform.childCount != 0 && slot.transform.GetChild(0).gameObject.GetComponent<Item>().itemID != inventory[i,0])){
                GameObject.Destroy(slot.transform.GetChild(0).gameObject);
            }
        }

        for(int i = 0; i < 3; i++){
            GameObject slot = GameObject.Find("invSlot" + i);
            if(inventory[i,0]!=0 && slot.transform.childCount == 0){
                Instantiate(itemKey[(int)inventory[i,0]], GameObject.Find("invSlot" + i).transform.position, new Quaternion(), GameObject.Find("invSlot" + i).transform);
            }
        }
    }

    void DropItem(){
        if(inventory[selectedInvSlot,0] != 0){
            GameObject item = Instantiate(itemKey[(int)inventory[selectedInvSlot,0]], new Vector2(transform.position.x, transform.position.y) + gameObject.GetComponent<Collider2D>().offset, new Quaternion());
            item.GetComponent<Item>().power = inventory[selectedInvSlot, 1];
        }
    }

    public bool isDying(){
        //Debug.Log("is dying is " + dying);
        return dying;
    }
}
