using UnityEngine;


//Name: Sam Johnson
//File: PlayerController.cs
//Purpose: Manage all player movement, input, and things affecting the player

public class PlayerController : MonoBehaviour
{
    public GameObject projectile; // prefab for projectile
    Vector3 movement; // direction of movement
    float moveSpeed = 500.0f; // how fast the player moves
    bool sprinting = false; // whether the player is sprinting this frame or not
    float sprintMult = 3.0f; // multiplier on how fast the player moves when sprinting
    float stamDrain = 30.0f; // how fast stamina drains per second of sprinting
    float stamRegen = 10.0f; // how fast stamina regenerates per second when not sprinting
    bool sneaking = false; // whether the player is sneaking
    float sneakMult = 0.5f; // how much slower the player moves while sneaking
    public float stamina = 100.0f; // how much stamina the player has
    public float thirst = 100.0f; // how much thirst the player has (100 = no thirst, 0 = completely thirsty)
    public float health = 100.0f; // health points
    public float Water = 100.0f; // how much water is in the player's water gun
    public float currentNoiseVolume = 0f; // per frame noise
    float sneakNoiseVolume = 4f; // how loud the player is while sneaking
    float walkNoiseVolume = 10f; // how loud the player is when walking
    float sprintNoiseVolume = 25f; // how lound the player is while sprinting
    LayerMask enemyMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyMask = LayerMask.GetMask("Enemy"); // set layer mask

        // Disable VSync to use target frameRate
        QualitySettings.vSyncCount = 0;

        // Set target frame rate to 120 FPS
        Application.targetFrameRate = 120;
    }

    // Update is called once per frame
    void Update()
    {
        //defining how the player should move this frame
        movement = new Vector3();
        if(Input.GetKey(KeyCode.W)){ // up
            movement += new Vector3(0,1f,0);
        } 
        if(Input.GetKey(KeyCode.S)){ // down
            movement += new Vector3(0,-1f,0);
        }
        if(Input.GetKey(KeyCode.A)){ // left
            movement += new Vector3(-1f,0,0);
        }
        if(Input.GetKey(KeyCode.D)){ // right
            movement += new Vector3(1f,0,0);
        }
        movement = Vector3.Normalize(movement)*moveSpeed; // normalize and set speed of movement in direction

        //is the player sprinting or sneaking? stamina regeneration if they aren't sprinting
        if(Input.GetKey(KeyCode.LeftShift) && movement != new Vector3()){
            sprinting = true;
        } else{
            sprinting = false;
            if(Input.GetKey(KeyCode.LeftControl)){
                sneaking = true;
            } else{
                sneaking = false;
            }
            if(stamina + stamRegen * Time.deltaTime < 100){
                stamina += stamRegen * Time.deltaTime;
            } else{
                stamina = 100.0f;
            }
            
        }

        //stamina drain and using movement
        if(sprinting && stamina - stamDrain * Time.deltaTime > 0){
            gameObject.GetComponent<Rigidbody2D>().AddForce(movement*sprintMult*Time.deltaTime);
            stamina = stamina - stamDrain * Time.deltaTime;
        } else if(sneaking){
            gameObject.GetComponent<Rigidbody2D>().AddForce(movement*sneakMult*Time.deltaTime);
        } else{
            gameObject.GetComponent<Rigidbody2D>().AddForce(movement*Time.deltaTime);
        }

        //thirst drain based on stamina
        if(thirst - Time.deltaTime * ((100 - stamina)/50 + 0.1f) > 0){
            thirst -= Time.deltaTime * ((100 - stamina)/50 + 0.1f);
        } else{
            thirst = 0;
        }

        //Water Gun shot
        if(Input.GetKeyDown(KeyCode.Space) && Water > 10.0f){
            Water -= 10;
            Vector3 dir = Vector3.Normalize(Camera.main.ScreenToWorldPoint(Input.mousePosition)-transform.position);
            GameObject proj = Instantiate(projectile, transform.position, new Quaternion());
            proj.GetComponent<Rigidbody2D>().AddForce(dir * 500.0f);
        }


        // alert enemies with noise - Skye
        currentNoiseVolume = 2f; // base noise volume
        if (sprinting && currentNoiseVolume < sprintNoiseVolume) {
            currentNoiseVolume = sprintNoiseVolume;
        } else if (movement.magnitude != 0 && currentNoiseVolume < walkNoiseVolume) {
            currentNoiseVolume = walkNoiseVolume;
        } else if (sneaking && currentNoiseVolume < sneakNoiseVolume) {
            currentNoiseVolume = sneakNoiseVolume;
        }
        // overlap circle to check for enemy tag - Skye
        Collider2D[] enemiesFound = Physics2D.OverlapCircleAll(transform.position, currentNoiseVolume, enemyMask);
        for(int i = 0; i < enemiesFound.Length; i++) {
            enemiesFound[i].gameObject.GetComponent<EnemyController>().recieveNoise(gameObject, new Vector2(transform.position.x, transform.position.y), true);
        }


        //Debug
        Debug.Log("Stamina: " + stamina + " Thirst: " + thirst);
        //Debug.Log("movement magnitude: " + movement.magnitude);
    }
}
