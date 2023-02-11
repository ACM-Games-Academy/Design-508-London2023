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
        SceneManager.LoadScene("Gameplay");
    }
    public void quit()
    {
        Application.Quit();
    }

    public void nextLevel()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        GameManager.currentMazes = new List<GameObject>();
        Reload();
    }

    public void pause()
    {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
    }

    public void unPause()
    {
        Time.timeScale = GameManager.instance.timeScale;
        pauseScreen.SetActive(false);
    }

}
