using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float timer = 0.25f;
    bool stopped = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(!stopped && timer <= 0){
            gameObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector3();
            stopped = true;
            timer = 0.25f;
        }
        else if (stopped && timer <= 0){
            GameObject.Destroy(gameObject);
        }
    }
}
