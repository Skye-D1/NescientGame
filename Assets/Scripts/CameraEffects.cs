using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //health vignette
        GameObject healthV = GameObject.Find("Vignette_Health");
        float health = GameObject.Find("Player").GetComponent<PlayerController>().health;

        healthV.GetComponent<SpriteRenderer>().color = new Color(1f, 0, 0, (100f-health)/100f);

        
        //stamina vignette
        GameObject stamV = GameObject.Find("Vignette_Stamina");
        float stamina = GameObject.Find("Player").GetComponent<PlayerController>().stamina;

        stamV.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, (100f-stamina)/100f);
    }
}
