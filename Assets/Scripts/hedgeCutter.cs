using UnityEngine;

public class hedgeCutter : MonoBehaviour
{
    public float totalDelay;
    float delay;
    public float distance;

    void Start()
    {
        delay = totalDelay;
    }

    // Update is called once per frame
    void Update()
    {
        delay -= Time.deltaTime;
        if(delay <= 0){
            Destroy(gameObject);
        }
        transform.position += transform.up * (delay/totalDelay) * (distance / 20);
    }
}
