using UnityEngine;

//Name: Sam Johnson
//File: hedgeCutter.cs
//Purpose: destroy and move the hedge cutter nimation that runs when the item is used

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
        //delete object if animation is complete
        delay -= Time.deltaTime;
        if(delay <= 0){
            Destroy(gameObject);
        }
        //move the animation forward during animation
        transform.position += transform.up * (delay/totalDelay) * (distance / 20);
    }
}
