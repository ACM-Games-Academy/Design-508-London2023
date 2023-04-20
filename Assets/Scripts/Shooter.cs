using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : Freezable
{
    public enum behaviours{search,aim};
    behaviours state;
    [SerializeField] string targetTag = "PlayerTargetPoint";
    [HideInInspector] public Collider targetCollider;
    [HideInInspector] public Transform pointer;

    Transform target;
    [Header("Detecting The Target")]
    public LayerMask WhatBlocksMyView;
    public float shootDistance;
    [Header("Aiming at the Target")]
    [SerializeField] bool isStatic;
    [SerializeField] bool rotateAtTarget;
    [SerializeField] Transform Xrotater;
    [SerializeField] Transform Yrotater;
    [SerializeField] float Xoffset;
    [SerializeField] float Yoffset; 
    [Header("Shooting at the Target")]
    public Transform Shootpoint;

    [SerializeField] GameObject bullet;
    public Vector3 aimVariation;
    [SerializeField][Tooltip("how long after the animation starts should the attack happen")] float shootAnimationDelay;
    [SerializeField] float shootCooldown;
    float originalCooldown;
    public float despawnTime;
    GameObject previousBullet;
    bool fire;

    // Start is called before the first frame update
    public virtual void Start()
    {
        target = GameObject.FindGameObjectWithTag(targetTag).transform;
        targetCollider = target.GetComponentInParent<PlayerController>().GetComponent<Collider>();
        pointer = Instantiate(new GameObject("pointer"),Shootpoint).transform;
        pointer.position = Shootpoint.position;
        StartCoroutine(ShootCooldown());
        originalCooldown = shootCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStatic)
        {
            pointer.position = Shootpoint.position;
        }
        Detection();
        if (state == behaviours.search)
        {
            //look around animation
            shootCooldown = 0;
        }
        if(state == behaviours.aim)
        {
            AimAtPlayer();
            shootCooldown = originalCooldown;
        }
        

    }

    void Detection()
    {
        pointer.LookAt(target);
        bool hit = Physics.Raycast(pointer.position, pointer.forward, out RaycastHit ray, shootDistance, WhatBlocksMyView);
        if (hit)
        {
            if (ray.collider == targetCollider && state != behaviours.aim)
            {
                state = behaviours.aim;
            }
            else if(ray.collider != targetCollider)
            {                
                state = behaviours.search;
            }
        }
        else
        {
            state = behaviours.search;
        }
        //Debug.DrawRay(Shootpoint.position, pointer.forward*detectionDistance, Color.blue);
    }

    void AimAtPlayer()
    {
        if (rotateAtTarget && !isRagdolling())
        {
            if(Xrotater != null)
            {
                float XAngle = 180 + Mathf.Atan2(Xrotater.position.y - target.position.y, Xrotater.position.z + Mathf.Abs(target.position.z)) * Mathf.Rad2Deg + Xoffset;//Find X angle toward target
                Xrotater.localRotation = Quaternion.Euler(XAngle, 0, 0);//Quaternion.Euler(Mathf.LerpAngle(Xrotater.localRotation.x, XAngle, rotationSpeed * Time.deltaTime), 0, 0);//Lerp rotation to target X angle
            }
            if(Yrotater != null)
            {
                float YAngle = 180 + Mathf.Atan2(Yrotater.position.x - target.position.x, Yrotater.position.z - target.position.z) * Mathf.Rad2Deg + Yoffset;//Find Y angle toward target
                Yrotater.localRotation = Quaternion.Euler(0, YAngle, 0);//Quaternion.Euler(0,Mathf.LerpAngle(Yrotater.localRotation.x, YAngle, rotationSpeed * Time.deltaTime), 0);//Lerp rotation to target Y angle
            }         
        }
    }

    public virtual void Shoot()
    {
        GameObject firedBullet = Instantiate(bullet, pointer.position, pointer.rotation);
        previousBullet = firedBullet;
        Physics.IgnoreCollision(firedBullet.GetComponent<Collider>(), transform.GetComponent<Collider>(), true);
        //Invoke("DestroyPrevious", despawnTime);
        fire = false;
    }

    void DestroyPrevious()
    {
        if (previousBullet != null)
        {
            Destroy(previousBullet);
        }
    }

    IEnumerator ShootCooldown()
    {
        float coolDownOver = Time.time + shootCooldown;
        fire = false;
        while(Time.time < coolDownOver)
        {
            //print("time: "+ Time.time);
            //print("end of cooldown: "+ (previousShotTime + shootCooldown).ToString());
            yield return new WaitForEndOfFrame();
        }
        if (state == behaviours.aim && !isRagdolling() && !isFrozen)
        {
            yield return new WaitForSeconds(shootAnimationDelay);
            Shoot();
        }
        yield return new WaitForEndOfFrame();
        StartCoroutine(ShootCooldown());
    }


    bool isRagdolling()
    {
        bool isRagdolled = false;
        if (TryGetComponent(out Ragdoll rs))
        {
            isRagdolled = (rs.ragdoll || rs.isGettingUp());
        }
        return isRagdolled;
    }
}


