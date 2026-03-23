using UnityEngine;

//Name: Skye Drury
//File: EnemySpawning.cs
//Purpose: make more enemy

public class enemySpawning : MonoBehaviour
{
    public GameObject spawnableEnemy;
    float timer;
    LayerMask playerMask;
    public GameObject player;

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
            //if (Physics2D.OverlapCircleAll(newPos, 20f, playerMask).Length > 0) {
            if (Vector3.Distance(player.transform.position, newPos) > 20f) {
                timer = Random.Range(0.12f, 0.20f); // todo: reset from debug level to 12f, 20f
                GameObject inst = Instantiate(spawnableEnemy, newPos, transform.rotation);
                inst.GetComponent<Rigidbody2D>().linearVelocity = new Vector2();
            }
        } else {
            timer -= Time.deltaTime;
        }
    }
}
