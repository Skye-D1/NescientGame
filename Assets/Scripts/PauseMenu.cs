/*
* Name: PauseMenu.cs
* Author: Aidan Gillette, Skye Drury
* Email: aidan.gillette, skye.drury
* Desc: Manages pause menu
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    /*
    Name: PauseGame
    Desc: Pauses the game
    */
    public void PauseGame()
    {
        Cursor.SetCursor(menuCursor, new Vector2(0,0), CursorMode.ForceSoftware);
        PauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        playerScript.isPaused = true;
    }

    /*
    Name: ResumeGame
    Desc: resumes the game
    */
    public void ResumeGame()
    {
        PauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        playerScript.isPaused = false;
        Cursor.SetCursor(gameCursor, new Vector2(32f,32f), CursorMode.ForceSoftware);
    }

    /*
    Name: QuitToMenu
    Desc: quits to the menu
    */
    public void QuitToMenu()
    {
        SceneManager.LoadScene("MainScene");
    }

    /*
    Name: LoadCredits
    Desc: loads the credits
    */
    public void LoadCredits()
    {
        SceneManager.LoadScene("CreditScene");
    }

}
