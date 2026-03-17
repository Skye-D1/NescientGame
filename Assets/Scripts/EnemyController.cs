using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // target movement location
    Vector2 target;
    bool isTargetPriority;
    public float echoRadius;
    LayerMask enemyMask = LayerMask.GetMask("Enemy");

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void recieveNoise(Vector2 newTarget, bool newTargetPriority)
    {
        // switch target if current is lower priority or null
        if (target == null || (!isTargetPriority && newTargetPriority)) {
            target = newTarget;
            isTargetPriority = newTargetPriority;
            echoNoise(newTarget, newTargetPriority);
        }
    }

    void echoNoise(Vector2 newTarget, bool newTargetPriority) {
        // overlap circle to check for enemy tag
        Collider2D[] enemiesFound = Physics2D.OverlapCircleAll(transform.position, echoRadius, enemyMask);
        for(int i = 0; i < enemiesFound.Length; i++) {
            enemiesFound[i].recieveNoise(newTarget, newTargetPriority);
        }
    }
}
