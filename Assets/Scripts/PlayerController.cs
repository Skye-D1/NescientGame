using UnityEngine;


//Name: Sam Johnson
//File: PlayerController.cs
//Purpose: Manage all player movement, input, and things affecting the player

public class PlayerController : MonoBehaviour
{
    Vector3 movement;
    float moveSpeed = 500.0f;
    bool sprinting = false;
    float sprintMult = 3.0f;
    float stamDrain = 30.0f;
    float stamRegen = 10.0f;
    bool sneaking = false;
    float sneakMult = 0.5f;
    public float stamina = 100.0f;
    public float thirst = 100.0f;
    public float health = 100.0f;
    float currentNoiseVolume = 0f; // per frame noise
    float sneakNoiseVolume = 4f;
    float walkNoiseVolume = 10f;
    float sprintNoiseVolume = 25f;
    LayerMask enemyMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyMask = LayerMask.GetMask("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        //defining how the player should move this frame
        movement = new Vector3();
        if(Input.GetKey(KeyCode.W)){
            movement += new Vector3(0,1f,0);
        } 
        if(Input.GetKey(KeyCode.S)){
            movement += new Vector3(0,-1f,0);
        }
        if(Input.GetKey(KeyCode.A)){
            movement += new Vector3(-1f,0,0);
        }
        if(Input.GetKey(KeyCode.D)){
            movement += new Vector3(1f,0,0);
        }
        movement = Vector3.Normalize(movement)*moveSpeed;

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

        //thirst
        if(thirst - Time.deltaTime * ((100 - stamina)/50 + 0.1f) > 0){
            thirst -= Time.deltaTime * ((100 - stamina)/50 + 0.1f);
        } else{
            thirst = 0;
        }

        // alert enemies with noise
        currentNoiseVolume = 2f; // base noise volume
        if (sprinting && currentNoiseVolume < sprintNoiseVolume) {
            currentNoiseVolume = sprintNoiseVolume;
        } else if (movement.magnitude != 0 && currentNoiseVolume < walkNoiseVolume) {
            currentNoiseVolume = walkNoiseVolume;
        } else if (sneaking && currentNoiseVolume < sneakNoiseVolume) {
            currentNoiseVolume = sneakNoiseVolume;
        }
        // overlap circle to check for enemy tag
        Collider2D[] enemiesFound = Physics2D.OverlapCircleAll(transform.position, currentNoiseVolume, enemyMask);
        for(int i = 0; i < enemiesFound.Length; i++) {
            enemiesFound[i].gameObject.GetComponent<EnemyController>().recieveNoise(gameObject, new Vector2(transform.position.x, transform.position.y), true);
        }


        //Debug
        Debug.Log("Stamina: " + stamina + " Thirst: " + thirst);
        //Debug.Log("movement magnitude: " + movement.magnitude);
    }
}
