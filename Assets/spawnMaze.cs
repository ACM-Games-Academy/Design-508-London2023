using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnMaze : MonoBehaviour
{
    GameObject maze;
    [SerializeField] bool left;
    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        if (left)
        {
            i = 1;
        }   
        Instantiate(GameManager.currentMazes[i], transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
