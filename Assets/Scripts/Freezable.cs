using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Freezable : MonoBehaviour
{
    bool isFrozen;

    //for rigidbodies
    float prevDrag;
    float prevAngDrag;

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
        print("FREEZE");
        if(TryGetComponent(out Rigidbody rb))
        {
            prevDrag = rb.drag;
            prevAngDrag = rb.angularDrag;
            rb.drag = 7;
            rb.angularDrag = 7;
            rb.useGravity = false;
        }
        if(TryGetComponent(out Animator ani))
        {
            ani.enabled = false;
        }
        if(TryGetComponent(out NavMeshAgent nm))
        {
            nm.enabled = false;
        }
        print("UNFREEZE");
    }

    public void unFreezeMethod()
    {
        if (TryGetComponent(out Rigidbody rb))
        {
            rb.drag = prevDrag;
            rb.angularDrag = prevAngDrag;
            rb.useGravity = true;
        }
        if (TryGetComponent(out Animator ani))
        {
            ani.enabled = true;
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
