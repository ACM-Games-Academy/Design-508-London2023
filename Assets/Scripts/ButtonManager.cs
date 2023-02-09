using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] GameObject pauseScreen;
    public static ButtonManager instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void titleScreen()
    {
        SceneManager.LoadScene("Menu");
    }
    public void start()
    {
        SceneManager.LoadScene("Level 1");
    }
    public void quit()
    {
        Application.Quit();
    }

    public void nextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public void pause()
    {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
    }

    public void unPause()
    {
        Time.timeScale = 2;
        pauseScreen.SetActive(false);
    }

}
