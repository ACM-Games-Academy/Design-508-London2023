using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fire : MonoBehaviour
{
    [SerializeField] float damageRadius;
    [SerializeField] float damagePerSecond;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Collider col in Physics.OverlapSphere(transform.position, damageRadius))          
        {
            if(col.TryGetComponent(out HealthManager health))
            {
                health.HealthChange(-damagePerSecond * Time.deltaTime);
            }
        }
    }
}
