using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ragdoll : MonoBehaviour
{
    private class BoneTransform
    {
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
    }
    
    NavMeshAgent agent;
    Collider coll;

    [SerializeField] Animator ani;
    [SerializeField] string getUpState;
    [SerializeField] float getUpDelay;
    [SerializeField] Transform hips;
    [SerializeField] List<Transform> bones;
    List<BoneTransform> ragdollTransforms;
    List<BoneTransform> animationTransforms;

    float startTime;
    public bool ragdoll;

    // Start is called before the first frame update
    void Start()
    {
        if(TryGetComponent(out NavMeshAgent a))
        {
            agent = a;
        }
        coll = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        RagdollCheck();
    }

    public void StartRagdoll()
    {
        ragdoll = true;
        RagdollCheck();
        Invoke("GetUp", getUpDelay);
    }

    public void RagdollCheck()
    {
        ani.enabled = !ragdoll;
        agent.enabled = !ragdoll;
        coll.enabled = !ragdoll;
    }
    

    public void GetUp()
    {
        ragdoll = false;
        Vector3 previousHipsPosition = hips.position;
        transform.position = hips.position;
        hips.position = previousHipsPosition;
        ani.Play(getUpState);
        RagdollCheck();
    }
}
