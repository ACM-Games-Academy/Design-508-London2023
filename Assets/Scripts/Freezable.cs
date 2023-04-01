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

            rb.drag = 7;
            rb.angularDrag = 7;
            rb.useGravity = false;
        }
        if(TryGetComponent(out Animator ani))
        {
            prevAniSpeed = ani.speed;
            ani.speed = 0.01f;
        }
        if(TryGetComponent(out NavMeshAgent nm))
        {
            nm.enabled = false;
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
        if (TryGetComponent(out Animator ani))
        {
            ani.speed = prevAniSpeed;
        }
        if(TryGetComponent(out NavMeshAgent nm))
        {
            nm.enabled = true;
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
