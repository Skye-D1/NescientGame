using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraEffects : MonoBehaviour
{
    public Volume volume;
    public Vignette vignette;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        volume.profile.TryGet(out vignette);
    }

    // Update is called once per frame
    void Update()
    {
        //health vignette
        GameObject healthV = GameObject.Find("Vignette_Health");
        float Health = GameObject.Find("Player").GetComponent<PlayerController>().Health;

        healthV.GetComponent<SpriteRenderer>().color = new Color(1f, 0, 0, (100f-Health)/100f);

        
        //stamina vignette
        if(!GameObject.Find("Player").GetComponent<PlayerController>().isDying()){
            //Debug.Log("camera check: " + GameObject.Find("Player").GetComponent<PlayerController>().isDying());
            float Stamina = GameObject.Find("Player").GetComponent<PlayerController>().Stamina;
            vignette.intensity.value = (100f-Stamina)/200f;
        }

        //stamV.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, (100f-Stamina)/100f);
    }
}
