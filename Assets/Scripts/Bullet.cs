using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed;

    [Header("What Happens On Collision?")]
    [SerializeField] MonoBehaviour collisionScript;
    [SerializeField] string collisionFunction;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(speed != 0)
        {
            rb.AddForce(transform.forward * speed * Time.deltaTime, ForceMode.Force);
        }
        
        
    }


    private void OnCollisionEnter(Collision collision)
    {
        collisionScript.Invoke(collisionFunction, 0);
    }

}
