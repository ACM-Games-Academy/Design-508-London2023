using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Freezable : MonoBehaviour
{
    bool isFrozen;
    float prevDrag;
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
            rb.drag = 5;
            rb.angularDrag = 5;
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
    }

    public void unFreezeMethod()
    {
        if (TryGetComponent(out Rigidbody rb))
        {
            rb.drag = prevDrag;
            rb.angularDrag = 0.01f;
            rb.useGravity = true;
        }
        if (TryGetComponent(out Animator ani))
        {
            ani.enabled = false;
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
