using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{   
    public void PlayGame ()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void QuitGame ()
    {
        Debug.Log ("QUIT");
        Application.Quit();
    }

    private void OnEnable()
    {
        GetComponentInChildren<Button>().Select();
    }
}
