using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject PauseUI;
    public GameObject HUD;
    public GameObject MenuUI;
    public GameObject MainMenu;
    public GameObject SettingsMenu;
    public GameObject FreeCam;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        FreeCam.SetActive(true);
        PauseUI.SetActive(false);
        HUD.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;

        Cursor.lockState = CursorLockMode.Locked;//needed to set the cursor back to how it was
        Cursor.visible = false;
    }

    void Pause()
    {
        FreeCam.SetActive(false);
        PauseUI.SetActive(true);
        HUD.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadMenu()
    {
        PauseUI.SetActive(false);
        MenuUI.SetActive(true);
        MainMenu.SetActive(true);
        SettingsMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
