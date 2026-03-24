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
        float Health = GameObject.Find("Player").GetComponent<PlayerController>().Health;

        healthV.GetComponent<SpriteRenderer>().color = new Color(1f, 0, 0, (100f-Health)/100f);

        
        //stamina vignette
        GameObject stamV = GameObject.Find("Vignette_Stamina");
        float Stamina = GameObject.Find("Player").GetComponent<PlayerController>().Stamina;

        stamV.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, (100f-Stamina)/100f);
    }
}
