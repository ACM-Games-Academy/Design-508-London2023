using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static Vector3 spawnPoint;
    public static bool superSpeed;
    public static bool laserVision;
    public static bool flight;
    PlayerController player;
    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (GameObject.FindGameObjectsWithTag("GameManager").Length > 1)
        {
            Destroy(this);
        }
        else
        {
            //first time loading the game
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
