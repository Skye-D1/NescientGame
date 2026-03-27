using UnityEngine;

//Name: Skye Drury
//File: LootContainerSpawn.cs
//Purpose: make more loot at container

public class LootContainerSpawn : MonoBehaviour
{
    public float itemTimer;
    LayerMask playerMask;
    public GameObject player;
    public GameObject[] itemSpawnTable;
    public bool isItemReady;
    public Vector3 spawnOffsets;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMask = LayerMask.GetMask("Player");
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // if item timer out and player not nearby, set ready and reset timer
        if (itemTimer < 0 && itemSpawnTable.Length > 0) {
            if (Vector3.Distance(player.transform.position, transform.position) > 10f) {
                isItemReady = true;
            }
            itemTimer = Random.Range(12f, 20f);
        } else {
            itemTimer -= Time.deltaTime;
        }
        
        // if player press e and in range
        if(isItemReady && Input.GetKeyDown(KeyCode.E) && Vector3.Distance(player.transform.position, transform.position) < 2f) {
            isItemReady = false;
            Instantiate(itemSpawnTable[Random.Range(0, itemSpawnTable.Length)], (transform.position + spawnOffsets), transform.rotation);
        }


    }
}
