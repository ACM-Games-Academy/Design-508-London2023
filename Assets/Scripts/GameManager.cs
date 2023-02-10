using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] float timeScale;
    [Header("Drag In Your Maze Prefab Here:")]
    public List<GameObject> mazes;

    [Header("Auto Defined In Script:")]
    public int playerNumber;
    public Material activePlayer;
    public Material inActivePlayer;
    public GameObject switchPrompt;
    public bool won;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        Time.timeScale = timeScale;
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
