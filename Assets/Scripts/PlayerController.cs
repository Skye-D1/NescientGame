using UnityEngine;


//Name: Sam Johnson
//File: PlayerController.cs
//Purpose: Manage all player movement, input, and things affecting the player

public class PlayerController : MonoBehaviour
{
    Vector3 movement;
    bool sprinting = false;
    float sprintMult = 3.0f;
    float stamDrain = 30.0f;
    float stamRegen = 10.0f;
    bool sneaking = false;
    float sneakMult = 0.5f;
    float stamina = 100.0f;
    float thirst = 100.0f;
    float health = 100.0f;
    float currentNoiseVolume = 0f; // per frame noise
    float sneakNoiseVolume = 4f;
    float walkNoiseVolume = 10f;
    float sprintNoiseVolume = 25f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
        movement = Vector3.Normalize(movement);

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
            gameObject.GetComponent<Rigidbody2D>().AddForce(movement*sprintMult);
            stamina = stamina - stamDrain * Time.deltaTime;
        } else if(sneaking){
            gameObject.GetComponent<Rigidbody2D>().AddForce(movement*sneakMult);
        } else{
            gameObject.GetComponent<Rigidbody2D>().AddForce(movement);
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
        }// incomplete


        //Debug
        Debug.Log("Stamina: " + stamina + " Thirst: " + thirst);
    }
}
