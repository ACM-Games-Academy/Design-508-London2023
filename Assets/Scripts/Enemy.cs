using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Freezable
{
    [Header("Optional")]
    [SerializeField] Animator ani;
    [HideInInspector] public NavMeshAgent agent;

    [Header("Range")]
    [SerializeField] float playerTargetRange;
    [SerializeField] bool showTargetRangeInSceneView;
    [SerializeField] float stoppingRange;
    [SerializeField] bool showStoppingRangeInSceneView;

    [Header("Player Detection")]
    [SerializeField] string playerTag = "PlayerTargetPoint";
    Transform player;
    Ragdoll ragdollScript;

    [Header("Death")]
    [SerializeField] GameObject bloodEffect;

    public virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag(playerTag).transform;
        if(TryGetComponent(out Ragdoll rs))
        {
            ragdollScript = rs;
        }
    }

    private void OnDrawGizmos()
    {
        if (showTargetRangeInSceneView)
        {
            ShowRange(playerTargetRange, new Color(1, 0, 0, 0.2f));
        }
        if (showStoppingRangeInSceneView)
        {
            ShowRange(stoppingRange, new Color(0.2f, 0.2f, 0.2f, 0.2f));
        }
    }

    void ShowRange(float range,Color gizmoColor)
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, range);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (Application.isPlaying)
        {
            if (!IsRagdolled())
            {  
                if (PlayerInRange())
                {
                    MoveToTarget(true);
                }
                else
                {
                    MoveToTarget(false);
                }
            }
            
        }
    }
    public virtual bool PlayerInRange()
    {
        //checking if the player is close enough to be seen
        bool closeEnough = (Vector3.Distance(transform.position, player.position) < playerTargetRange);

        return closeEnough && !tooClose();//if close enough, but not too close, return true.
    }

    public bool tooClose()
    {
        return (Vector3.Distance(transform.position, player.position) <= stoppingRange);
    }

    public virtual void Die()
    {
        if(ragdollScript != null)
        {
            Transform hips = ani.GetBoneTransform(HumanBodyBones.Hips);
            Instantiate(bloodEffect, hips.position, hips.rotation);
            ragdollScript.getBackUp = false;
            if (!IsRagdolled())
            {
                ragdollScript.StartRagdoll();
            }
        }
        if (TryGetComponent(out DropWeaponOnDeath Dr))
        {
            Dr.Drop();
        }
    }

    public void MoveToTarget(bool toggle)
    {
        agent.destination = player.position;
        agent.isStopped = !toggle;
        if (ani != null)
        {
            ani.SetBool("Walking", toggle);
            ani.SetBool("Aiming", !toggle);
        }
    }

    bool IsRagdolled()
    {
        if(ragdollScript != null)
        {
            return ragdollScript.ragdoll;
        }
        else
        {
            return (false);
        }
    }
}
