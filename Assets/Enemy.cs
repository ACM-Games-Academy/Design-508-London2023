using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] float targetRange;
    [SerializeField] string playerTag;
    NavMeshAgent agent;

    //[Header("Ranged Settings")]
    bool isRanged;
    Shooter shootScript;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if(TryGetComponent(out Shooter s))
        {
            shootScript = s;
            isRanged = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = GameObject.FindGameObjectWithTag(playerTag).transform.position;
        if (isRanged)
        {
            if(shootScript.state == Shooter.behaviours.aim)
            {
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
            }
        }
        
    }
}
