using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject MenuUI;
    public GameObject PlayerUI;
    
    public void PlayGame ()
    {
        MenuUI.SetActive(false);
        PlayerUI.SetActive(true);
    }

    public void QuitGame ()
    {
        Debug.Log ("QUIT");
        Application.Quit();
    }
}
