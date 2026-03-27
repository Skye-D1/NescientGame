using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//Name: Sam Johnson
//File: CameraEffects.cs
//Purpose: Manage all post-processing effects

public class CameraEffects : MonoBehaviour
{
    public Volume volume;
    public Vignette vignette;
    ColorAdjustments saturation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out saturation);
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
            vignette.intensity.value = (100f-Stamina)/175f;
        }

        //thirst desaturation
        float Thirst = GameObject.Find("Player").GetComponent<PlayerController>().Thirst;
        saturation.saturation.value = -(100f-Thirst);
    }
}
