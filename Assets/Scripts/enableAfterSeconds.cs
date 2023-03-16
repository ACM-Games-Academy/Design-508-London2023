using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enableAfterSeconds : MonoBehaviour
{
    [SerializeField] float enableTime;
    [SerializeField] List<GameObject> gameObjects = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        Invoke("enable", enableTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void enable()
    {
        foreach (GameObject ob in gameObjects)
        {
            ob.SetActive(true);
        }
    }
}
