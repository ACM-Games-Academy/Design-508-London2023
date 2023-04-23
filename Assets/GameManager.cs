using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static Vector3 spawnPoint;
    public static bool superSpeed;
    public static bool laserVision;
    public static bool flight;
    public static PlayerController player;
    public static GameManager instance;
    [HideInInspector]public TextMeshProUGUI powerTutorial;

    //waves
    public GameObject waveUI;
    // Start is called before the first frame update
    void Awake()
    {
        //power unlocks
        powerTutorial = GameObject.FindGameObjectWithTag("tutorialText").GetComponent<TextMeshProUGUI>();
        powerTutorial.transform.parent.gameObject.SetActive(false);


        //Wave UI
        waveUI = GameObject.FindGameObjectWithTag("waveUI");
        waveUI.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (GameObject.FindGameObjectsWithTag("GameManager").Length > 1)
        {
            Destroy(this);
        }
        else
        {
            //first time loading the game
            instance = this;
            DontDestroyOnLoad(this);
            spawnPoint = player.transform.position;
            superSpeed = player.superSpeed;
            laserVision = player.laserVision;
            flight = player.flight;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
