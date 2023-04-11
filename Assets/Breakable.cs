using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(HealthManager))]
public class Breakable : MonoBehaviour
{
    public GameObject inactivePieces;
    public float breakForce;

    void Awake()
    {
        inactivePieces.SetActive(false);
    }

    public void BreakIt()
    {
        inactivePieces.SetActive(true);
        inactivePieces.transform.SetParent(null);
        foreach(Rigidbody rb in inactivePieces.GetComponentsInChildren<Rigidbody>())
        {
            Vector3 force = (rb.transform.position - transform.position) * breakForce;
        }
        Destroy (gameObject);
    }
}
