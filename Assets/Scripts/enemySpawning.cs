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
    public float mapHeight; // default 54f for alpha map
    public float mapWidth; // default 120f for alpha map
    public GameObject[] itemSpawnTable;
    public float[] enemySpawnDelayRange; // min and max delay values
    public int totalEnemiesSpawned = 0;
    public int popDensityCap; // was 15

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
                // if population below limit
                Collider2D[] enemiesFound = Physics2D.OverlapCircleAll(new Vector2(transform.position.x + newPos.x, transform.position.y + newPos.y), 20.0f, LayerMask.GetMask("Enemy"));
                if (enemiesFound.Length < popDensityCap) {
                    // reset timer
                    enemyTimer = Random.Range(enemySpawnDelayRange[0], enemySpawnDelayRange[1]);
                    // spawn enemy
                    GameObject inst = Instantiate(spawnableEnemy, newPos, transform.rotation);
                    inst.GetComponent<Rigidbody2D>().linearVelocity = new Vector2();
                    totalEnemiesSpawned++;
                }
            }
        } else {
            // progress timer
            enemyTimer -= Time.deltaTime;
        }

        // if item timer out and new position valid, spawn item and reset timer
        /*if (itemTimer < 0 && itemSpawnTable.Length > 0) {
            Vector3 newPos = new Vector3(Random.Range(-(mapWidth/2f), (mapWidth/2f)), Random.Range(-(mapHeight/2f), (mapHeight/2f)), 0);
            if (Vector3.Distance(player.transform.position, newPos) > 20f) {
                itemTimer = Random.Range(20f, 4000f);
                Instantiate(itemSpawnTable[Random.Range(0, itemSpawnTable.Length)], newPos, transform.rotation);
            }
        } else {
            itemTimer -= Time.deltaTime;
        }*/
    }
}
