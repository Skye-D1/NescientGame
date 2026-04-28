//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Name: Aidan Gillette, Skye Drury
//File: PauseMenu.cs
//Purpose: Manages pause menu

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseMenuPanel;
    public bool isPaused = false;
    public bool isMainMenu = true;
    MainMenu mainMenu;

    PlayerController playerScript; // the player controlscript for purposes
    public Texture2D menuCursor;
    public Texture2D gameCursor;


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
        //activate on escape
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

    //pauses the game
    public void PauseGame()
    {
        Cursor.SetCursor(menuCursor, new Vector2(0,0), CursorMode.ForceSoftware);
        PauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        playerScript.isPaused = true;
    }

    //resumes the game
    public void ResumeGame()
    {
        PauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        playerScript.isPaused = false;
        Cursor.SetCursor(gameCursor, new Vector2(32f,32f), CursorMode.ForceSoftware);
    }

    //quits to the menu
    public void QuitToMenu()
    {
        SceneManager.LoadScene("MainScene");
    }

    //loads the credits
    public void LoadCredits()
    {
        SceneManager.LoadScene("CreditScene");
    }

}
