using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float timer = 0.25f;
    bool stopped = false;
    Vector3 randomDir = new Vector3();
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
            timer = Random.Range(0.25f, 0.5f);
        }
        else if (stopped && timer <= 0){
            GameObject.Destroy(gameObject);
        }
        if(stopped){
            transform.position -= randomDir;
            randomDir = new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f), 0);
            transform.position += randomDir;
        }
    }
}
