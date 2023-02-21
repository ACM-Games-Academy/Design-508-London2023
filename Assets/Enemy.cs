using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode]
public class Enemy : MonoBehaviour
{
    [SerializeField] float playerTargetRange;
    [SerializeField] bool showRangeInSceneView;
    [SerializeField] string playerTag;
    NavMeshAgent agent;
    Transform player;

    //[Header("Ranged Settings")]
    bool isRanged;
    Shooter shootScript;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag(playerTag).transform;
        if (TryGetComponent(out Shooter s))
        {
            shootScript = s;
            isRanged = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (showRangeInSceneView)
        {
            Gizmos.color = new Color(1, 0, 0, 0.2f);
            Gizmos.DrawSphere(transform.position, playerTargetRange);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying)
        {
            bool targetPlayer = (Vector3.Distance(transform.position, player.position) < playerTargetRange);
            agent.isStopped = !targetPlayer;
            if (targetPlayer)
            {
                agent.destination = player.position;
                if (isRanged)
                {
                    if (shootScript.state == Shooter.behaviours.aim)
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
    }
}
