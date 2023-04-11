using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyingEnemyNavigation : MonoBehaviour
{
    // Start is called before the first frame update
    Transform movePositionTransform;
    [SerializeField] Transform helicopter;
    float chopperHeight;

    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        chopperHeight = helicopter.transform.position.y;
        movePositionTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        navMeshAgent.destination = movePositionTransform.position;
        helicopter.position = new Vector3(helicopter.position.x, chopperHeight, helicopter.position.z);
    }


}
