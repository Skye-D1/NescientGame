//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Name: Aidan Gillette
//File: PauseMenu.cs
//Purpose: Manages pause menu

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseMenuPanel;
    public bool isPaused = false;
    public bool isMainMenu = true;
    MainMenu mainMenu;

    PlayerController playerScript; // the player controlscript for purposes


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PauseMenuPanel.SetActive(false);
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        mainMenu = GameObject.Find("MainMenuThing").GetComponent<MainMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        PauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        playerScript.isPaused = true;
    }


    public void ResumeGame()
    {
        PauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        playerScript.isPaused = false;
    }


    public void QuitToMenu()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void LoadCredits()
    {
        SceneManager.LoadScene("CreditScene");
    }

}
