using UnityEngine;

//Name: Skye Drury
//File: noiseCircleController.cs
//Purpose: make noise pulse and delete self

public class noiseCircleController : MonoBehaviour
{
    public float noiseRange;
    float timer = 0;
    float opacityRate = -0.75f; // -1.5f
    float offset; // 1f is start at full opacity, determined on start, works with 0.5f
    float lifetime = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = new Vector3(0.01f, 0.01f, 1f);
        offset = 0.5f;//Mathf.Lerp(0.2f, 0.6f, (noiseRange / 20f));
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        transform.localScale = new Vector3(noiseRange * (2f * timer), noiseRange * (2f * timer), 1f);
        if(timer >= lifetime*0.8f){
            opacityRate = -4f;
            offset = 2f;
        }
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,(opacityRate*timer + offset));
        //Debug.Log("noisecircle: " + (opacityRate*timer + offset));
    

        if (timer > lifetime) {
            GameObject.Destroy(gameObject);
        }
    }
}
