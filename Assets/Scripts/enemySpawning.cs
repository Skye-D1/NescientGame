using UnityEngine;

//Name: Skye Drury
//File: EnemySpawning.cs
//Purpose: make more enemy

public class enemySpawning : MonoBehaviour
{
    public GameObject spawnableEnemy;
    float enemyTimer;
    float itemTimer;
    LayerMask playerMask;
    public GameObject player;
    float mapHeight = 54f;
    float mapWidth = 120f;
    public GameObject[] itemSpawnTable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMask = LayerMask.GetMask("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // if enemy timer out and new position valid, spawn enemy and reset timer
        if (enemyTimer < 0) {
            Vector3 newPos = new Vector3(Random.Range(-(mapWidth/2f), (mapWidth/2f)), Random.Range(-(mapHeight/2f), (mapHeight/2f)), 0);
            if (Vector3.Distance(player.transform.position, newPos) > 20f) {
                enemyTimer = Random.Range(12f, 20f);
                GameObject inst = Instantiate(spawnableEnemy, newPos, transform.rotation);
                inst.GetComponent<Rigidbody2D>().linearVelocity = new Vector2();
            }
        } else {
            enemyTimer -= Time.deltaTime;
        }

        // if item timer out and new position valid, spawn item and reset timer
        if (itemTimer < 0 && itemSpawnTable.Length > 0) {
            Vector3 newPos = new Vector3(Random.Range(-(mapWidth/2f), (mapWidth/2f)), Random.Range(-(mapHeight/2f), (mapHeight/2f)), 0);
            if (Vector3.Distance(player.transform.position, newPos) > 20f) {
                itemTimer = Random.Range(12f, 20f);
                Instantiate(itemSpawnTable[Random.Range(0, itemSpawnTable.Length)], newPos, transform.rotation);
            }
        } else {
            itemTimer -= Time.deltaTime;
        }
    }
}
