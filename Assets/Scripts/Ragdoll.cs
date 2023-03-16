using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ragdoll : MonoBehaviour
{
    private class BoneTransform
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    }
    
    NavMeshAgent agent;
    Collider coll;

    [SerializeField] Animator ani;
    [SerializeField] string getUpState;
    [SerializeField] string getUpClipName;
    [SerializeField] float getUpDelay;
    Transform hips;
    Transform[] bones;
    Rigidbody[] ragdollBones;
    BoneTransform[] ragdollTransforms;
    BoneTransform[] animationTransforms;

    [SerializeField] float boneResetTime;
    float elapsedResetTime;

    public bool ragdoll;
    public bool getBackUp;
    bool resettingBones;

    // Start is called before the first frame update
    void Start()
    {
        getBackUp = true;
        if(TryGetComponent(out NavMeshAgent a))
        {
            agent = a;
        }
        coll = GetComponent<Collider>();
        hips = ani.GetBoneTransform(HumanBodyBones.Hips);
        //
        //bones = new Transform[ragdollBones.Length];
        //for(int x = 0; x < ragdollBones.Length; x++)
        //{
        //    bones[x] = ragdollBones[x].transform;
        //}
        bones = hips.GetComponentsInChildren<Transform>();
        ragdollBones = hips.GetComponentsInChildren<Rigidbody>();
        //I don't think I need this here
        animationTransforms = new BoneTransform[bones.Length];
        ragdollTransforms = new BoneTransform[bones.Length];
        for(int x = 0; x < bones.Length; x++)
        {
            animationTransforms[x] = new BoneTransform();
            ragdollTransforms[x] = new BoneTransform();
        }
        //
        StartAnimationTransforms();
    }

    // Update is called once per frame
    void Update()
    {
        if (resettingBones)
        {
            TransitionToAnimation();
        }
    }

    public void StartRagdoll()
    {
        ragdoll = true;
        RagdollCheck();
        if (getBackUp)
        {
            Invoke("GetUp", getUpDelay);
        }      
    }

    public void RagdollCheck()
    {
        ani.enabled = !ragdoll;
        agent.enabled = !ragdoll;
        coll.enabled = !ragdoll;
        foreach(Rigidbody rb in ragdollBones)
        {
            rb.isKinematic = !ragdoll;
        }
    }

    BoneTransform[] PopulateTransforms(Transform[] bns)
    {
        List<BoneTransform> boneTransforms = new List<BoneTransform>();
        foreach(Transform t in bns)
        {
            BoneTransform b = new BoneTransform();
            b.Position = t.localPosition;
            b.Rotation = t.localRotation;
            boneTransforms.Add(b);
        }
        return boneTransforms.ToArray();
    }

    void StartAnimationTransforms()
    {
        Vector3 oldPosition = ani.transform.position;
        Quaternion oldRotation = ani.transform.rotation;
        foreach(AnimationClip clip in ani.runtimeAnimatorController.animationClips)
        {
            if(clip.name == getUpClipName)
            {
                clip.SampleAnimation(ani.gameObject, 0);
                animationTransforms = PopulateTransforms(bones);
            }
        }
        ani.transform.position = oldPosition;
        ani.transform.rotation = oldRotation;
    }

    public void GetUp()
    {
        HipRotationReset();
        HipPositionReset();
        ragdollTransforms = PopulateTransforms(bones);
        resettingBones = true;
    }


    void HipPositionReset()
    {
        Vector3 previousHipsPosition = hips.position;
        transform.position = hips.position;

        Vector3 offset = animationTransforms[0].Position;
        offset.y = 0;
        offset = transform.rotation * offset;
        transform.position -= offset;

        if(Physics.Raycast(hips.position,Vector3.down, out RaycastHit ray))
        {
            transform.position = new Vector3(transform.position.x, ray.point.y, transform.position.z);
        }
        hips.position = previousHipsPosition;
    }

    void HipRotationReset()
    {
        Vector3 previousHipsPosition = hips.position;
        Quaternion previousHipsRotation = hips.rotation;

        Vector3 direction = hips.up * -1;
        direction.y = 0;
        direction.Normalize();
        transform.rotation *= Quaternion.FromToRotation(transform.forward, direction);

        hips.position = previousHipsPosition;
        hips.rotation = previousHipsRotation;
    }

    public void TransitionToAnimation()
    {
        elapsedResetTime += Time.deltaTime;
        float elapsedPercentage = elapsedResetTime / boneResetTime;
        for (int x = 0; x < bones.Length; x++)
        {
            bones[x].localPosition = Vector3.Lerp(ragdollTransforms[x].Position, animationTransforms[x].Position, elapsedResetTime/ boneResetTime);
            bones[x].localRotation = Quaternion.Lerp(ragdollTransforms[x].Rotation, animationTransforms[x].Rotation,elapsedResetTime / boneResetTime);
        }
        if(elapsedPercentage >= 1)
        {
            ragdoll = false;
            resettingBones = false;
            RagdollCheck();
            ani.Play(getUpState);
            elapsedResetTime = 0;
        }

    }

    public bool isGettingUp()
    {
        return ani.GetCurrentAnimatorStateInfo(0).IsName(getUpState);
    }
}
