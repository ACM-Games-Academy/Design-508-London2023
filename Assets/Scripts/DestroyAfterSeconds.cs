using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField] float seconds;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Gone", seconds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Gone()
    {
        Destroy(gameObject);
    }
}
