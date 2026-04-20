using UnityEngine;

public class destroyAfterDelay : MonoBehaviour
{
    public float delay;
    // Update is called once per frame
    void Update()
    {
        delay -= Time.deltaTime;
        if(delay <= 0){
            Destroy(gameObject);
        }
    }
}
