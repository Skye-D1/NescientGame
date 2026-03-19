using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // target movement location
    public Vector2 target;
    float moveSpeed = 200f;
    Vector3 movement = new Vector3();
    public bool isTargetPriority;
    bool didTargetUpdate;
    public float echoRadius;
    LayerMask enemyMask;
    Collider2D[] enemiesFound; // enemies found wjen circle

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyMask = LayerMask.GetMask("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        didTargetUpdate = false; // allow one (1) target update per frame
        // move towards target
        if (target != null) {
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
            isTargetPriority = newTargetPriority;
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
