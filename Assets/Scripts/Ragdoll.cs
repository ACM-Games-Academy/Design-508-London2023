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
    float originalMass;

    [SerializeField] Animator ani;
    [HideInInspector]public bool ragdoll;
    [SerializeField] bool testRagdoll;
    [Header("Getting Up")]
    [HideInInspector] public bool getBackUp;
    [SerializeField] bool neverGetBackUp;
    [SerializeField] string getUpState = "Getting Up";
    [SerializeField] string getUpClipName = "Getting Up";
    [SerializeField] float getUpDelay;
    Transform hips;
    Transform[] bones;
    Rigidbody[] ragdollBones;
    BoneTransform[] ragdollTransforms;
    BoneTransform[] animationTransforms;

    [SerializeField] float boneResetTime;
    float elapsedResetTime;
    bool resettingBones;
    bool throwable;
    Throwable throwScript;

    // Start is called before the first frame update
    void Start()
    {
        getBackUp = true;

        //GET COMPONENTING
        if(TryGetComponent(out NavMeshAgent a))
        {
            agent = a;
        }
        coll = GetComponent<Collider>();
        hips = ani.GetBoneTransform(HumanBodyBones.Hips);
        bones = hips.GetComponentsInChildren<Transform>();
        ragdollBones = hips.GetComponentsInChildren<Rigidbody>();
        if (hips.TryGetComponent(out Throwable t))
        {
            throwable = true;
            throwScript = t;
            t.enabled = false;
        }
        if (TryGetComponent(out Rigidbody rb))
        {
            originalMass = rb.mass;
        }
        foreach(Rigidbody rdBone in ragdollBones)
        {
            if(rdBone.transform != hips)
            {
                rdBone.gameObject.AddComponent<Freezable>();
            }
            
        }      

        //I don't think I need this here
        animationTransforms = new BoneTransform[bones.Length];
        ragdollTransforms = new BoneTransform[bones.Length];
        for(int x = 0; x < bones.Length; x++)
        {
            animationTransforms[x] = new BoneTransform();
            ragdollTransforms[x] = new BoneTransform();
        }

        StartAnimationTransforms();
        TogglePhysics(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (resettingBones)
        {
            TransitionToAnimation();
        }
        if (testRagdoll)
        {
            StartRagdoll();
            testRagdoll = false;
        }
    }

    public void StartRagdoll()
    {
        ragdoll = true;
        RagdollCheck();
        if (getBackUp && !neverGetBackUp)
        {
            Invoke("GetUp", getUpDelay);
        }      
    }

    public void RagdollCheck()
    {
        ani.enabled = !ragdoll;
        if(agent != null)
        {
            agent.enabled = !ragdoll;
        }
        if (throwScript != null)
        {
            throwScript.enabled = ragdoll;
        }
        TogglePhysics(ragdoll);
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
        if (!CancelGetUp() && !neverGetBackUp)
        {
            print("running get up");
            HipRotationReset();
            HipPositionReset();
            ragdollTransforms = PopulateTransforms(bones);
            resettingBones = true;
            if (throwScript != null)
            {
                throwScript.enabled = false;
            }
        }
        else if(!neverGetBackUp)
        {
            //print("delayed get up for " + 1 + " second");
            Invoke("GetUp", 5);
        }

    }

    bool CancelGetUp()
    {
        //print("checking for cancel");
        if (TryGetComponent(out HealthManager hm))//if dead, don't get back up
        {
            if (hm.dead)
            {
                //print("no no no i am dead");
                neverGetBackUp = true;
                return true;              
            }
        }
        if (hips.GetComponent<Freezable>().isFrozen)//if time is frozen don't get up
        {
            //print("no no no, i am frozen");
            return true;
        }
        Debug.DrawRay(hips.position, Vector3.down.normalized,Color.cyan);
        if (!Physics.Raycast(hips.position,Vector3.down.normalized, 1))//if not grounded, pretty much
        {
            //print("no no no i am in the air");
            return true;
        }
        if (throwScript != null)//if the player is holding thou art, lest we not stand up
        {
            //print("no no no, i am being held by the player");
            return throwScript.beingHeld;
        }
        return false;
        
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

    public void TogglePhysics(bool toggle)
    {
        foreach(Rigidbody rb in ragdollBones)
        {
            rb.isKinematic = !toggle;
            if(rb.TryGetComponent(out Collider c))
            {
                c.enabled = toggle;
            }
        }
        if (TryGetComponent(out Rigidbody thisrb))
        {
            if (toggle)
            {
                thisrb.mass = 1;
            }
            else
            {
                thisrb.mass = originalMass;                
            }
            thisrb.useGravity = !toggle;
            thisrb.isKinematic = toggle;
        }
        coll.enabled = !toggle;
    }
}
