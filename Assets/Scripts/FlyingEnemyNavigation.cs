using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyingEnemyNavigation : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform movepositionTransform;

    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        navMeshAgent.destination = movepositionTransform.position;
    }


}
