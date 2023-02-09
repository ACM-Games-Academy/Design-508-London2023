using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int playerNumber;
    public Material activePlayer;
    public Material inActivePlayer;
    public GameObject switchPrompt;
    public bool won;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        Time.timeScale = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("escape"))
        {
            if(Time.timeScale != 0)
            {
                 ButtonManager.instance.pause();
            }
            else
            {
                ButtonManager.instance.unPause();
            }
           
        }
    }
}
