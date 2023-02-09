using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnOnChild : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        transform.GetChild(0).gameObject.SetActive(true); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
