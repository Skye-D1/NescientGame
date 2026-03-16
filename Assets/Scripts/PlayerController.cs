using UnityEngine;

public class PlayerController : MonoBehaviour
{
    bool sprinting = false;
    float sprintMult = 3.0f;
    float stamina = 100.0f;
    float stamDrain = 30.0f;
    float stamRegen = 10.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = new Vector3();
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

        if(Input.GetKey(KeyCode.LeftShift)){
            sprinting = true;
        } else{
            sprinting = false;
            if(stamina + stamRegen * Time.deltaTime < 100){
                stamina += stamRegen * Time.deltaTime;
            } else{
                stamina = 100.0f;
            }
            
        }

        if(sprinting && stamina - stamDrain * Time.deltaTime > 0){
            gameObject.GetComponent<Rigidbody2D>().AddForce(movement*sprintMult);
            stamina = stamina - stamDrain * Time.deltaTime;
        } else{
            gameObject.GetComponent<Rigidbody2D>().AddForce(movement);
        }
        Debug.Log("Current stamina: " + stamina);
    }
}
