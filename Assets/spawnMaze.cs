using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnMaze : MonoBehaviour
{
    GameObject maze;

    // Start is called before the first frame update
    void Start()
    {
        List<GameObject> mazeList = GameManager.instance.mazes;
        maze = mazeList[Random.Range(0, mazeList.Count)];
        Instantiate(maze, transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
