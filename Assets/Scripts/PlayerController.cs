using UnityEngine;


//Name: Sam Johnson, Skye Drury
//File: PlayerController.cs
//Purpose: Manage all player movement, input, and things affecting the player

public class PlayerController : MonoBehaviour
{
    public GameObject projectile; // prefab for projectile
    public GameObject noiseCircle; // reference to circle for noise range debug
    Vector3 movement; // direction of movement
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
    float sneakNoiseVolume = 4f; // how loud the player is while sneaking
    float walkNoiseVolume = 10f; // how loud the player is when walking
    float sprintNoiseVolume = 25f; // how loud the player is while sprinting
    LayerMask enemyMask;
    int selectedInvSlot = 0;
    float[,] inventory = new float[3,2];
    public GameObject[] itemKey;
    float hitCooldown = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyMask = LayerMask.GetMask("Enemy"); // set layer mask

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
        //defining how the player should move this frame
        movement = new Vector3();
        movement.x = Input.GetAxisRaw("Horizontal");
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

        //Water Gun shot
        if(Input.GetButtonDown("Fire1") && Water > 10.0f){
            //Water -= 10;
            Vector3 dir = Vector3.Normalize(Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0,0,10) -transform.position);
            
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
        if(Input.GetKeyDown(KeyCode.E)){
            Collider2D[] itemsFound = Physics2D.OverlapCircleAll(transform.position, 2.0f, LayerMask.GetMask("Item"));
            if(itemsFound.Length > 0){
                inventory[selectedInvSlot, 0] = itemsFound[0].gameObject.GetComponent<Item>().itemID;
                inventory[selectedInvSlot, 1] = itemsFound[0].gameObject.GetComponent<Item>().power;
                GameObject.Destroy(itemsFound[0].gameObject);
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
                if(inventory[selectedInvSlot, 0] == 1){
                    //Water Bottle
                    if(100f-Water >= inventory[selectedInvSlot, 1]){
                        Water += inventory[selectedInvSlot, 1];
                        inventory[selectedInvSlot, 0] = 0;
                        inventory[selectedInvSlot, 1] = 0;
                    } else if(100f-Water < inventory[selectedInvSlot, 1]){
                        inventory[selectedInvSlot, 1] -= 100f-Water;
                        Water = 100f;
                    }
                } else if(inventory[selectedInvSlot, 0] == 2){
                    //Health Item
                    if(100f-Health >= inventory[selectedInvSlot, 1]){
                        Health += inventory[selectedInvSlot, 1];
                        inventory[selectedInvSlot, 0] = 0;
                        inventory[selectedInvSlot, 1] = 0;
                    } else if(100f-Health < inventory[selectedInvSlot, 1]){
                        inventory[selectedInvSlot, 1] -= 100f-Health;
                        Health = 100f;
                    }

                } else if(inventory[selectedInvSlot, 0] == 3){
                    //Hedge Clippers
                }
            }
        }

        UpdateInventory();

        // alert enemies with noise - Skye
        currentNoiseVolume = 2f; // base noise volume
        if (sprinting && movement.magnitude != 0) {
            currentNoiseVolume = sprintNoiseVolume;
        } else if (movement.magnitude != 0 && !sneaking) {
            currentNoiseVolume = walkNoiseVolume;
        } else if (movement.magnitude != 0 && sneaking) {
            currentNoiseVolume = sneakNoiseVolume;
        }
        // overlap circle to check for enemy tag - Skye
        Collider2D[] enemiesFound = Physics2D.OverlapCircleAll(transform.position, currentNoiseVolume, enemyMask);
        for(int i = 0; i < enemiesFound.Length; i++) {
            enemiesFound[i].gameObject.GetComponent<EnemyController>().recieveNoise(new Vector2(transform.position.x, transform.position.y), true);
        }

        noiseCircle.transform.localScale = new Vector3(currentNoiseVolume, currentNoiseVolume, 1f); // debug? maybe

        //cooldown on how often enemies can hit player
        if(hitCooldown > 0){
            hitCooldown -= Time.deltaTime;
        }
        //does enemy hit player?
        if(hitCooldown <= 0){
            Collider2D enemyCollisions = Physics2D.OverlapCircle(transform.position, 2.0f, LayerMask.GetMask("Enemy"));
            if(enemyCollisions != null && (Health - 5) >= 0){
                Health -= 5;
                hitCooldown = 0.5f;
            }
        }
        
    }

    void UpdateInventory(){
        for(int i = 0; i < 3; i++){
            GameObject slot = GameObject.Find("invSlot" + i);
            if(slot.transform.childCount != 0){
                GameObject.Destroy(slot.transform.GetChild(0).gameObject);
            }
            
        }

        for(int i = 0; i < 3; i++){
            if(inventory[i,0]!=0){
                Instantiate(itemKey[(int)inventory[i,0]], GameObject.Find("invSlot" + i).transform.position, new Quaternion(), GameObject.Find("invSlot" + i).transform);
            }
        }
    }

    void DropItem(){
        if(inventory[selectedInvSlot,0] != 0){
            Instantiate(itemKey[(int)inventory[selectedInvSlot,0]], transform.position, new Quaternion());
        }
    }
}
