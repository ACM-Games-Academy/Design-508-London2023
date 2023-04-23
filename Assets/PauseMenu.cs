using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject PauseUI;
    public GameObject HUD;
    public GameObject FreeCam;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject controlsMenu;
    
    void Update()
    {
        if (Input.GetButtonDown("Pause") && !PlayerController.playerHealth.dead)
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
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);

        Time.timeScale = 1f;
        GameIsPaused = false;

        Cursor.lockState = CursorLockMode.Locked;//needed to set the cursor back to how it was
        Cursor.visible = false;
    }

    void Pause()
    {
        PauseUI.GetComponentInChildren<Button>().Select();
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
        SceneManager.LoadScene("Main Menu");
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
