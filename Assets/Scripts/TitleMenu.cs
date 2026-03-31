using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Name: Skye Drury
//File: TitleMenu.cs
//Purpose: menu for title screen scene

public class TitleMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Button pbutton = GameObject.Find("Play button").GetComponent<Button>();
        pbutton.onClick.AddListener(playFunction);
        
        Button creditsbutton = GameObject.Find("Credits").GetComponent<Button>();
        creditsbutton.onClick.AddListener(creditsFunction);
        
        Button quitbutton = GameObject.Find("Quit").GetComponent<Button>();
        quitbutton.onClick.AddListener(quitFunction);
        
        Debug.Log("setup complete");
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Return)) {
            SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        }
    }

    // load main scene
    public void playFunction() {
        Debug.Log("meow");
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }

    // 
    public void creditsFunction() {
        Debug.Log("credits time");
    }

    // quit game
    public void quitFunction() {
        Application.Quit();
    }

    
}
