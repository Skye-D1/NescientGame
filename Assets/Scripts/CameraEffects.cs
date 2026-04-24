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
    ColorAdjustments desaturation;
    GameObject mainMenuPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainMenuPanel = GameObject.Find("MainMenuPanel");
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out desaturation);
    }

    // Update is called once per frame
    void Update()
    {
        //health vignette
        GameObject healthV = GameObject.Find("Vignette_Health");
        float Health = GameObject.Find("Player").GetComponent<PlayerController>().Health;

        healthV.GetComponent<SpriteRenderer>().color = new Color(1f, 0, 0, (100f-Health)/100f);

        
        //stamina vignette
        if(!GameObject.Find("Player").GetComponent<PlayerController>().isDying() && !mainMenuPanel.activeSelf){
            float Stamina = GameObject.Find("Player").GetComponent<PlayerController>().Stamina;
            vignette.intensity.value = (100f-Stamina)/175f;
        }
    }
}
