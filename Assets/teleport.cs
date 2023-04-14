using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleport : MonoBehaviour
{
    [SerializeField] Transform destination;
    [SerializeField] string teleportTag = "Player";
    [SerializeField] float teleportDelay;
    Transform transformToMove;
    // Start is called before the first frame update
    void Awake()
    {
        transformToMove = GameObject.FindGameObjectWithTag(teleportTag).transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Teleport()
    {
        transformToMove.position = destination.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == teleportTag)
        {
            Invoke("Teleport", 1);
        }
    }
}
