/*
* Name: MainMenu.cs
* Author: Skye Drury
* Email: skye.drury
* Desc: Manages main menu
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject MainMenuPanel;
    GameObject MMVignette;
    //public bool isPaused = false;

    PlayerController playerScript; // the player controlscript for purposes
    PauseMenu pauseMenu;
    float vScale = 1f;
    GameObject menuThing;

    public Texture2D menuCursor;
    public Texture2D gameCursor;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuThing = GameObject.Find("MenuThing");
        Time.timeScale = 0f; // start paused
        MainMenuPanel.SetActive(true);
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        playerScript.isPaused = true;
        pauseMenu = GameObject.Find("MenuThing").GetComponent<PauseMenu>();
        menuThing.SetActive(false);
        MMVignette = GameObject.Find("MMVignette");
        Cursor.SetCursor(menuCursor, new Vector2(0,0), CursorMode.ForceSoftware);
    }

    // Update is called once per frame
    void Update()
    {
        MMVignette.transform.localScale = new Vector3(vScale, vScale, vScale);
        if (!MainMenuPanel.activeSelf && vScale < 10f) {
            vScale += Time.deltaTime;
        } else if (vScale > 10f) {
            MMVignette.SetActive(false);
        }
    }

    /*
    Name: StartGame
    Desc: starts the game
    */
    public void StartGame()
    {
        MainMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        playerScript.isPaused = false;
        pauseMenu.isMainMenu = false;
        menuThing.SetActive(true);
        Cursor.SetCursor(gameCursor, new Vector2(32f,32f), CursorMode.ForceSoftware);
    }

    /*
    Name: QuitGame
    Desc: quits the game
    */
    public void QuitGame()
    {
        Application.Quit();
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
