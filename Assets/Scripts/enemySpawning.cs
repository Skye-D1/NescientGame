using UnityEngine;

//Name: Skye Drury
//File: EnemySpawning.cs
//Purpose: make more enemy

public class enemySpawning : MonoBehaviour
{
    public GameObject enemy;
    float timer;
    LayerMask playerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMask = LayerMask.GetMask("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < 0) {
            Vector3 newPos = new Vector3(Random.Range(-60f, 60f), Random.Range(-40f, 40f), 0);
            if (Physics2D.OverlapCircleAll(newPos, 20f, playerMask).Length > 0) {
                timer = Random.Range(12f, 20f);
                GameObject inst = Instantiate(enemy, newPos, transform.rotation);
                inst.GetComponent<Rigidbody2D>().linearVelocity = new Vector2();
            }
        } else {
            timer -= Time.deltaTime;
        }
    }
}
