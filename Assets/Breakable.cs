using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public GameObject Breaks;
    public float breakForce;

    void Update()
    {
        
    }

    public void BreakIt()
    {
        GameObject Break = Instantiate(Breaks, transform.position, transform.rotation);

        foreach(Rigidbody rb in Break.GetComponentsInChildren<Rigidbody>())
        {
            Vector3 force = (rb.transform.position - transform.position) * breakForce;
        }
        Destroy (gameObject);
    }
}
