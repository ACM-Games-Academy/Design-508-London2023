using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyingEnemyNavigation : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform movepositionTransform;
    [SerializeField] Transform helicopter;
    float chopperHeight;

    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        print(navMeshAgent.gameObject.name);
        chopperHeight = helicopter.transform.position.y;
    }

    private void Update()
    {
        navMeshAgent.destination = movepositionTransform.position;
        helicopter.position = new Vector3(helicopter.position.x, chopperHeight, helicopter.position.z);
    }


}
