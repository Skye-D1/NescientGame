using UnityEngine;

//Name: Skye Drury
//File: EnemySpawning.cs
//Purpose: make more enemy

public class enemySpawning : MonoBehaviour
{
    public GameObject[] spawnableEnemies;
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
    public int forceSpawnCount; // force a number of zombies to spawn over the next frames
    public float spawnrateChangeFactor; // how much to increase spawnrate
    public bool doSpawning;
    public GameObject[] tutorialObjects;
    public Vector3[] tutorialTriggers;
    int tutorialIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMask = LayerMask.GetMask("Player");
        if (tutorialObjects.Length > 0) {
            tutorialObjects[tutorialIndex].SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if enemy timer out and new position valid, spawn enemy and reset timer
        if ((enemyTimer < 0 || forceSpawnCount > 0) && doSpawning) {
            if (forceSpawnCount > 0) {
                forceSpawnCount -= 1;
            }
            Vector3 newPos = new Vector3(Random.Range(-(mapWidth/2f), (mapWidth/2f)), Random.Range(-(mapHeight/2f), (mapHeight/2f)), 0);
            if (Vector3.Distance(player.transform.position, newPos) > 20f) {
                // if population below limit
                Collider2D[] enemiesFound = Physics2D.OverlapCircleAll(new Vector2(transform.position.x + newPos.x, transform.position.y + newPos.y), 20.0f, LayerMask.GetMask("Enemy"));
                if (enemiesFound.Length < popDensityCap) {
                    // reset timer
                    enemyTimer = Random.Range(enemySpawnDelayRange[0], enemySpawnDelayRange[1]);
                    // spawn enemy
                    GameObject inst = Instantiate(spawnableEnemies[Random.Range(0, spawnableEnemies.Length)], newPos, transform.rotation);
                    inst.GetComponent<Rigidbody2D>().linearVelocity = new Vector2();
                    totalEnemiesSpawned++;
                }
            }
        } else {
            // progress timer
            enemyTimer -= Time.deltaTime;
        }
        enemySpawnDelayRange[0] /= (1 + (Time.deltaTime * (spawnrateChangeFactor/60f)));
        enemySpawnDelayRange[1] /= (1 + (Time.deltaTime * (spawnrateChangeFactor/60f)));
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.transform.name == "Player") {
            tutorialObjects[tutorialIndex].SetActive(false);
            tutorialIndex++;
            tutorialObjects[tutorialIndex].SetActive(true);

            // move trigger to next pos
        }
    }
}
