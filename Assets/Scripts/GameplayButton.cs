using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayButton : MonoBehaviour
{
    [SerializeField] string function;
    [SerializeField] float delay;
    [SerializeField] ButtonManager bm;
    [SerializeField] bool clickable;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("a");
        GameObject colliderObject = collision.gameObject;
        if (colliderObject.tag == "Player")
        {
            print("b");
            bm.Invoke(function, delay);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        GameObject colliderObject = collision.gameObject;
        if (colliderObject.tag == "Player")
        {
            bm.CancelInvoke();
        }
    }

    private void OnMouseDown()
    {
        bm.Invoke(function, 0);
    }
}
