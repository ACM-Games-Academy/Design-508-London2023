using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Freezable : MonoBehaviour
{
    [HideInInspector] public bool isFrozen;

    //for rigidbodies
    float prevDrag;
    float prevAngDrag;
    Vector3 prevVelocity;
    bool wasUsingGravity;

    //for animators
    float prevAniSpeed;

    //for navmesh agents
    float prevAgentSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void incrementVelocity(Vector3 velocity)
    {
        if(TryGetComponent(out Rigidbody rb))
        {
            prevVelocity += velocity;
        }       
    }

    public void freezeMethod()
    {
        isFrozen = true;
        if(TryGetComponent(out Rigidbody rb))
        {
            prevDrag = rb.drag;
            prevAngDrag = rb.angularDrag;
            prevVelocity = Vector3.zero;
            incrementVelocity(rb.velocity);

            rb.drag = 3;
            rb.angularDrag = 3;
            rb.useGravity = false;
        }
        foreach(Animator ani in GetComponentsInChildren<Animator>())
        {
            ani.enabled = false;
        }
        if(TryGetComponent(out NavMeshAgent nm))
        {
            bool defineSpeed = true;
            if(TryGetComponent(out Ragdoll rs))
            {
                defineSpeed = !rs.ragdoll;
            }
            if (defineSpeed)
            {
                prevAgentSpeed = nm.speed;
                print(prevAgentSpeed);
                nm.speed = 0;
            }
        }
    }

    public void unFreezeMethod()
    {
        isFrozen = false;
        if (TryGetComponent(out Rigidbody rb))
        {
            rb.drag = prevDrag;
            rb.angularDrag = prevAngDrag;
            rb.velocity = prevVelocity;
            rb.useGravity = true;
        }
        foreach (Animator ani in GetComponentsInChildren<Animator>())
        {
            ani.enabled = true;
        }
        if (TryGetComponent(out NavMeshAgent nm))
        {
            if(prevAgentSpeed != 0)
            {
                nm.speed = prevAgentSpeed;
            }           
        }
        if (TryGetComponent(out Ragdoll rs))
        {
            if (rs.ragdoll)
            {
                rs.StartRagdoll();
            }          
        }
    }

    bool BeingHeld()
    {
        if(GetComponentInParent<Throwable>() != null)
        {
            Throwable ts = GetComponentInParent<Throwable>();
            if (ts.beingHeld)
            {
                return true;
            }
        }
        return false;
    }

    private void OnEnable()
    {
        PlayerController.freezeEvent += freezeMethod;
        PlayerController.unFreezeEvent += unFreezeMethod;
    }

    private void OnDisable()
    {
        PlayerController.freezeEvent -= freezeMethod;
        PlayerController.unFreezeEvent -= unFreezeMethod;
    }
}
