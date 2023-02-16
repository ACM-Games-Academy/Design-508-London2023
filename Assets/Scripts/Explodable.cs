using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explodable : MonoBehaviour
{
    [SerializeField] GameObject explosion;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Explode()
    {
        GameObject blast =  Instantiate(explosion);
        blast.transform.position = transform.position; 
        Destroy(gameObject);
    }
}
