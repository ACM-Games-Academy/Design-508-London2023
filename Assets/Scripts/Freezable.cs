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

    public void freezeMethod()
    {

        if(TryGetComponent(out Rigidbody rb))
        {
            prevDrag = rb.drag;
            prevAngDrag = rb.angularDrag;
            wasUsingGravity = rb.useGravity;
            prevVelocity = rb.velocity;

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
            prevAgentSpeed = nm.speed;
            print(prevAgentSpeed);
            nm.speed = 0;
        }
    }

    public void unFreezeMethod()
    {
        if (TryGetComponent(out Rigidbody rb))
        {
            rb.drag = prevDrag;
            rb.angularDrag = prevAngDrag;
            rb.useGravity = wasUsingGravity;
            rb.velocity = prevVelocity;
        }
        foreach (Animator ani in GetComponentsInChildren<Animator>())
        {
            ani.enabled = true;
        }
        if (TryGetComponent(out NavMeshAgent nm))
        {
            nm.speed = prevAgentSpeed;
        }
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
