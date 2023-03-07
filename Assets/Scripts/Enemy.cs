using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode]
public class Enemy : MonoBehaviour
{
    [SerializeField] Animator ani;
    NavMeshAgent agent;
    Collider coll;
    float normalMoveSpeed;

    [Header("Range")]
    [SerializeField] float playerTargetRange;
    [SerializeField] bool showRangeInSceneView;

    [Header("Player Detection")]
    [SerializeField] string playerTag;
    Transform player;
    Ragdoll ragdollScript;

    //[Header("Ranged Settings")]
    bool isRanged;
    Shooter shootScript;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag(playerTag).transform;
        coll = GetComponent<Collider>();
        ragdollScript = GetComponent<Ragdoll>();
        normalMoveSpeed = agent.speed;
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
            if (!ragdollScript.ragdoll)
            {
                bool withinRange = PlayerInRange();
                if (withinRange)
                {
                    Agro();
                }
                else
                {
                    //idle animation
                }
            }
            
        }
    }
    public bool PlayerInRange()
    {
        //checking if the player is close enough to be targeted
        bool targetPlayer = (Vector3.Distance(transform.position, player.position) < playerTargetRange);
        agent.isStopped = (!targetPlayer);//stopping movement if within shoot range                                        
        return targetPlayer;
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public void Agro()
    {
        agent.destination = player.position;
        if (isRanged)
        {
            if (shootScript.state == Shooter.behaviours.aim)
            {
                agent.speed = 0;
                ani.SetBool("Aiming", true);
                ani.SetBool("Walking", false);
            }
            else
            {
                agent.speed = normalMoveSpeed;
                ani.SetBool("Walking", true);
                ani.SetBool("Aiming", false);
            }
        }
    }
}
