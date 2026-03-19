using UnityEngine;

//Name: Skye Drury
//File: EnemyController.cs
//Purpose: Manage enemy movement, recieving and communicating noises, and other enemy behavior

public class EnemyController : MonoBehaviour
{
    public Vector2 target; // target movement location
    float moveSpeed = 200f; // speed of movement
    Vector3 movement = new Vector3(); // current movement direction
    public bool targetPriority; // whether the target is high priority (player noise)
    bool didTargetUpdate; // whether the enemy has updated target this frame
    public float echoRadius; // radius to echo recieved noise to others
    LayerMask enemyMask; // layermask for finding other enemies
    Collider2D[] enemiesFound; // enemies found wjen circle
    public bool isStatic; // if enemy does not move for tutorial
    public bool isLeader; // if enemy will create wander targets

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyMask = LayerMask.GetMask("Enemy");
        target = new Vector2(transform.position.x, transform.position.y); // target own position on start
        // determine if leader
        if (Random.value() > 0.9) { // todo: check proximity for other leaders
            isLeader = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        didTargetUpdate = false; // allow one target update per frame
        // move towards target
        if (!isStatic) {
            movement = Vector3.Normalize(new Vector3(target.x, target.y, 0) - transform.position) * moveSpeed;
            gameObject.GetComponent<Rigidbody2D>().AddForce(movement * Time.deltaTime);
        }
    }

    public void recieveNoise(Vector2 newTarget, bool newTargetPriority)
    {
        // switch target if new is priority or current is null
        if ((target == null || newTargetPriority) && !didTargetUpdate) {
            didTargetUpdate = true;
            target = newTarget;
            targetPriority = newTargetPriority;
            if (newTargetPriority) { // echo to others if priority high
                echoNoise(newTarget, newTargetPriority);
            }
        }
    }

    void echoNoise(Vector2 newTarget, bool newTargetPriority) {
        // overlap circle to check for enemy tag
        enemiesFound = Physics2D.OverlapCircleAll(transform.position, echoRadius, enemyMask); // the aforementioned circle
        Debug.Log("echo :3 " + newTargetPriority);
        for(int i = 0; i < enemiesFound.Length; i++) {
            enemiesFound[i].gameObject.GetComponent<EnemyController>().recieveNoise(newTarget, newTargetPriority);
        }
    }
}
