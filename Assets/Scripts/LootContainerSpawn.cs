/*
* Name: LootContainerSpawn.cs
* Author: Skye Drury
* Email: skye.drury
* Desc: Makes more loot at containers
*/

using UnityEngine;

public class LootContainerSpawn : MonoBehaviour
{
    public float itemTimer;
    LayerMask playerMask;
    public GameObject player;
    public GameObject[] itemSpawnTable;
    public bool isItemReady;
    public Vector3 spawnOffsets;
    public Sprite openSprite; // set to alternate
    Sprite closedSprite; // set from current on start
    SpriteRenderer spriteRenderer;
    public bool restrictAccess; // whether restrict to box trigger
    public bool destroyOnUse; // for single use container
    public GameObject[] disabledOnOpen; // disable some objects on open
    bool isPlayerInTrigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        playerMask = LayerMask.GetMask("Player");
        player = GameObject.Find("Player");
        closedSprite = spriteRenderer.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        // if item timer out and player not nearby, set ready and reset timer
        if (itemTimer < 0 && itemSpawnTable.Length > 0) {
            if (Vector3.Distance(player.transform.position, transform.position) > 15f) {
                isItemReady = true;
                spriteRenderer.sprite = closedSprite;
            }
            itemTimer = Random.Range(12f, 20f);
        } else {
            itemTimer -= Time.deltaTime;
        }
        
        // if player press e and in range
        if(isItemReady && Input.GetKeyDown(KeyCode.E) && Vector3.Distance(player.transform.position, transform.position) < 2f && (!restrictAccess || isPlayerInTrigger)) {
            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayClip(24, false);
            isItemReady = false;
            spriteRenderer.sprite = openSprite;
            Instantiate(itemSpawnTable[Random.Range(0, itemSpawnTable.Length)], (transform.position + spawnOffsets), transform.rotation);
            for (int i = 0; i < disabledOnOpen.Length; i++) {
                disabledOnOpen[i].gameObject.SetActive(false);
            }
            if (destroyOnUse) {
                // particles here maybe
                GameObject.Destroy(gameObject);
            }
        }
    }

    // if the player enters the trigger, set isPlayerInTrigger to true
    void OnTriggerEnter2D(Collider2D found) {
        // if thing is player, isPlayerInTrigger set to true
        if (found.gameObject.transform.name.Contains("Player")) {
            isPlayerInTrigger = true;
        }
    }

    // if the player exits the trigger, set isPlayerInTrigger to false
    void OnTriggerExit2D(Collider2D found) {
        // if thing is player, isPlayerInTrigger set to false
        if (found.gameObject.transform.name.Contains("Player")) {
            isPlayerInTrigger = false;
        }
    }
}
