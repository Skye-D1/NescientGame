using UnityEngine;

public class Projectile : MonoBehaviour
{
    float timer = 0.25f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0){
            GameObject.Destroy(gameObject);
        }
    }
}
