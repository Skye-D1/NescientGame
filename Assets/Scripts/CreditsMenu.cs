/*
* Name: CreditsMenu.cs
* Author: Skye Drury
* Email: skye.drury
* Desc: Manages credits UI
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsMenu : MonoBehaviour
{
    /*
    Name: LoadMain
    Desc: loads the main menu scene
    */
    public void LoadMain()
    {
        SceneManager.LoadScene("MainScene");
    }
}
