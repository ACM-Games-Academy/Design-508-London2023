using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitbox : Explodable
{
    [Header("Melee Properties")]
    public float meleeDamage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     private void OnTriggerEnter(Collider other)
    {
        GameObject collisionObject = other.gameObject;
        if(collisionObject.TryGetComponent(out HealthManager health))
        {
            health.HealthChange(-meleeDamage);
        }
        if(collisionObject.TryGetComponent(out Ragdoll rd))
        {
            rd.StartRagdoll();
        }
        if (collisionObject.TryGetComponent(out Rigidbody rb))
        {
            //ExplosionForce(rb);
            rb.AddForce(transform.forward* force, ForceMode.Impulse);
            print(rb.gameObject.name);
        }
    }
}
