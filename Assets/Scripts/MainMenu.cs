//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//Name: Skye Drury
//File: MainMenu.cs
//Purpose: Manages main menu

public class MainMenu : MonoBehaviour
{
    public GameObject MainMenuPanel;
    //public bool isPaused = false;

    PlayerController playerScript; // the player controlscript for purposes
    PauseMenu pauseMenu;
    Vignette vignette;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 0f; // start paused
        MainMenuPanel.SetActive(true);
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        playerScript.isPaused = true;
        pauseMenu = GameObject.Find("MenuThing").GetComponent<PauseMenu>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (vignette == null) {
            vignette = GameObject.Find("Main Camera").GetComponent<CameraEffects>().vignette;
            vignette.intensity.value = 1f;
        }
        if (MainMenuPanel.activeSelf) {
            if(vignette.intensity.value < 1f){
                vignette.intensity.value += Time.deltaTime;
            }else if(vignette.smoothness.value < 1f){
                vignette.smoothness.value += Time.deltaTime;
            }
        } else {
            if(vignette.intensity.value > 0f){
                vignette.intensity.value -= Time.deltaTime;
            }else if(vignette.smoothness.value > 0f){
                vignette.smoothness.value -= Time.deltaTime;
            }
        }
    }

    public void StartGame()
    {
        MainMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        playerScript.isPaused = false;
        pauseMenu.isMainMenu = false;
    }


    public void QuitGame()
    {
        Application.Quit();
    }

}
