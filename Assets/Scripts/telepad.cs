using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class telepad : MonoBehaviour
{
    bool touching;
    GameObject cargo;
    [SerializeField] Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(touching && Input.GetKeyDown("e"))
        {
            int index = 0;
            if(transform.parent.GetChild(index) == transform)
            {
                index = 1;
            }
            cargo.transform.position = transform.parent.GetChild(index).position + offset;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject colliderObject = collision.gameObject;
        if (colliderObject.tag == "Player")
        {
            touching = true;
            cargo = colliderObject;
            GameManager.instance.switchPrompt.GetComponent<TextMeshProUGUI>().text = "Press 'E' to Teleport";
            GameManager.instance.switchPrompt.SetActive(true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        GameObject colliderObject = collision.gameObject;
        if (colliderObject.tag == "Player")
        {
            touching = false;
            GameManager.instance.switchPrompt.GetComponent<TextMeshProUGUI>().text = "Press 'E' to switch to object";
            GameManager.instance.switchPrompt.SetActive(false);
            cargo = null;
        }
    }
}
