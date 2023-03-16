using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[RequireComponent(typeof(BoxCollider))]
public class DialogueExecute : MonoBehaviour
{
    [SerializeField] string setOffTag = "Player";
    [SerializeField] string blockName;
    Flowchart flowchart;
    // Start is called before the first frame update
    void Start()
    {
        flowchart = FindObjectOfType<Flowchart>();
        GetComponent<BoxCollider>().isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == setOffTag && flowchart != null)
        {
            if (flowchart.FindBlock(blockName) != null)
            {
                flowchart.ExecuteBlock(blockName);
            }
            else
            {
                Debug.Log("Block Name Not Found");
            }
            
        }
        else
        {
            Debug.Log("Flowchart Not Found");
        }
    }
}
