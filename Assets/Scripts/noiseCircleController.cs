using UnityEngine;

//Name: Skye Drury
//File: noiseCircleController.cs
//Purpose: make noise pulse and delete self

public class noiseCircleController : MonoBehaviour
{
    public float noiseRange;
    float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = new Vector3(0.01f, 0.01f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        transform.localScale = new Vector3(noiseRange * (2f * timer), noiseRange * (2f * timer), 1f);
        if (timer > 0.5f) {
            GameObject.Destroy(gameObject);
        }
    }
}
