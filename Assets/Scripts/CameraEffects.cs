/*
* Name: CameraEffects.cs
* Author: Sam Johnson
* Email: samuel.johnson
* Desc: Manages all post processing effects
*/

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraEffects : MonoBehaviour
{
    GameObject mainMenuPanel;
    GameObject staminaBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainMenuPanel = GameObject.Find("MainMenuPanel");
    }

    // Update is called once per frame
    void Update()
    {
        //health vignette
        GameObject healthV = GameObject.Find("Vignette_Health");
        float Health = GameObject.Find("Player").GetComponent<PlayerController>().Health;

        healthV.GetComponent<SpriteRenderer>().color = new Color(1f, 0, 0, Mathf.Clamp((100f-Health)/60f, 0f, 1f));

        
        //stamina bar
        float stamina = GameObject.Find("Player").GetComponent<PlayerController>().Stamina;
        float center = staminaBar.transform.parent.position.x;
        staminaBar.transform.localScale = new Vector3((100f - stamina)/100f * 10, 1, 1);
        staminaBar.transform.position = new Vector3(center - center/2 + staminaBar.transform.localScale.x/2, staminaBar.transform.parent.position.y, 0);
    }
}
