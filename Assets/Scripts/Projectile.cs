/*
* Name: Projectile.cs
* Author: Sam Johnson
* Email: samuel.johnson
* Desc: Manage player's projectiles
*/

using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float timer = 0.25f;
    bool stopped = false;
    Vector3 randomDir = new Vector3();
    Vector3 origin;
    float maxDistance = 0f;
    bool firstFrame = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //on first frame of its existence it checks if it will hit anything marked as too thin so that it doesn't pass through it.
        if(firstFrame){
            RaycastHit2D hit = Physics2D.Raycast(transform.position, gameObject.GetComponent<Rigidbody2D>().linearVelocity, 5f, LayerMask.GetMask("ProjectileBlocker"));
            if(hit){
                maxDistance = hit.distance;
            }
            origin = transform.position;
        }

        //if it is passed the max distance, delete it
        if(maxDistance != 0 && Vector3.Distance(origin, transform.position) > maxDistance){
            timer = 0f;
        }
        //update timer
        timer -= Time.deltaTime;
        //if timer is done, stop it
        if(!stopped && timer <= 0){
            gameObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector3();
            stopped = true;
            timer = Random.Range(0.25f, 0.5f);
            gameObject.GetComponent<layerSetDynamic>().enabled = false;
            gameObject.GetComponent<TrailLayerSetDynamic>().enabled = false;
        }
        else if (stopped && timer <= 0){
            GameObject.Destroy(gameObject);
        }
        //wiggle around for splash effect
        if(stopped){
            transform.position -= randomDir;
            randomDir = new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f), 0);
            transform.position += randomDir;
        }
    }

    //hits a thing when it collides with it and stops the projectile
    void OnTriggerEnter2D(Collider2D collider){
        if(!stopped && !collider.gameObject.CompareTag("Player") && !collider.gameObject.CompareTag("Projectile")){
            //Debug.Log(collider.gameObject.name);
            timer = 0f;
            if(collider.gameObject.CompareTag("Enemy")){
                collider.gameObject.GetComponent<EnemyController>().plantify();
            }
        }
    }
}
