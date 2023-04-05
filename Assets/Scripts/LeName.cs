using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LeName : MonoBehaviour //sorry
{

    public GameObject UpgradesMenu;
    

    // Start is called before the first frame update
    void Start()
    {
       UpgradesMenu.SetActive(false);


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FilthySubroutine()
    {
        OpenMenu();

    }

    public void OpenMenu()
    {
        UpgradesMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PlayerController.disableInputs = false;
    }

    public void CloseMenu()
    {
        UpgradesMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        PlayerController.disableInputs = true;
    }

    public void Option1()
    {

    }

    public void Option2()
    {

    }

    public void Option3()
    {

    }
}
