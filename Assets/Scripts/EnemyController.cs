using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // target movement location
    public Vector2 target;
    public bool isTargetPriority;
    public float echoRadius;
    LayerMask enemyMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyMask = LayerMask.GetMask("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void recieveNoise(Vector2 newTarget, bool newTargetPriority)
    {
        // switch target if new is priority or null
        if (target == null || newTargetPriority) {
            target = newTarget;
            isTargetPriority = newTargetPriority;
            echoNoise(newTarget, newTargetPriority);
        }
    }

    void echoNoise(Vector2 newTarget, bool newTargetPriority) {
        // overlap circle to check for enemy tag
        Collider2D[] enemiesFound = Physics2D.OverlapCircleAll(transform.position, echoRadius, enemyMask);
        for(int i = 0; i < enemiesFound.Length; i++) {
            enemiesFound[i].gameObject.GetComponent<EnemyController>().recieveNoise(newTarget, newTargetPriority);
        }
    }
}
