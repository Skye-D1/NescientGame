//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//Name: Aidan Gillette
//File: PauseMenu.cs
//Purpose: Manages pause menu

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseMenuPanel;
    public bool isPaused = false;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PauseMenuPanel.SetActive(false);
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
    }


    public void ResumeGame()
    {
        PauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }


    public void QuitGame()
    {
        Application.Quit();
    }

}
