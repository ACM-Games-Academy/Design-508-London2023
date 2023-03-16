using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spinMe : MonoBehaviour
{
    [Range(10,1000)]
    [SerializeField] float roticerySpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.right*roticerySpeed * Time.deltaTime);
    }
}
