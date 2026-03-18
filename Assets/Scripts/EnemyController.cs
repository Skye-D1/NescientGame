using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // target movement location
    public Vector2 target;
    public bool isTargetPriority;
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
        
    }

    public void recieveNoise(GameObject source, Vector2 newTarget, bool newTargetPriority)
    {
        // switch target if new is priority or null
        if ((target == null || newTargetPriority) && source != gameObject) {
            target = newTarget;
            isTargetPriority = newTargetPriority;
            echoNoise(newTarget, newTargetPriority);
        }
    }

    void echoNoise(Vector2 newTarget, bool newTargetPriority) {
        // overlap circle to check for enemy tag
        enemiesFound = Physics2D.OverlapCircleAll(transform.position, echoRadius, enemyMask);
        Debug.Log("echo :3");
        for(int i = 0; i < enemiesFound.Length; i++) {
            enemiesFound[i].gameObject.GetComponent<EnemyController>().recieveNoise(gameObject, newTarget, newTargetPriority);
        }
    }
}
