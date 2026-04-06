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
    public GameObject MMVignette;
    //public bool isPaused = false;

    PlayerController playerScript; // the player controlscript for purposes
    PauseMenu pauseMenu;
    float vScale = 1f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 0f; // start paused
        MainMenuPanel.SetActive(true);
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        playerScript.isPaused = true;
        pauseMenu = GameObject.Find("MenuThing").GetComponent<PauseMenu>();
        MMVignette = GameObject.Find("MMVignette");
        
    }

    // Update is called once per frame
    void Update()
    {
        MMVignette.transform.localScale = new Vector3(vScale, vScale, vScale);
        if (MainMenuPanel.activeSelf) {
        } else {
            vScale += Time.deltaTime;
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
