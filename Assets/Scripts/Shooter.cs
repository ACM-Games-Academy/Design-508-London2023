using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : Freezable
{
    public enum behaviours{search,aim};
    public behaviours state;
    [SerializeField] string targetTag = "PlayerTargetPoint";
    Collider targetCollider;
    Transform target;
    Transform pointer;
    [Header("Detecting The Target")]
    [SerializeField] LayerMask WhatBlocksMyView;
    [SerializeField] [Range(0.01f,100)]float shootDistance;
    [Header("Aiming at the Target")]
    [SerializeField] bool isStatic;
    [SerializeField] bool rotateAtTarget;
    [SerializeField] Transform Xrotater;
    [SerializeField] Transform Yrotater;
    [SerializeField] float Xoffset;
    [SerializeField] float Yoffset; 
    [Header("Shooting at the Target")]
    [SerializeField] Transform Shootpoint;
    enum bulletType { projectile,hitscan,melee};
    [SerializeField] float hitscanDamage;
    [SerializeField] bulletType mode;
    [SerializeField] GameObject bullet;
    [SerializeField] Vector3 aimVariation;
    [SerializeField][Tooltip("how long after the animation starts should the attack happen")] float shootAnimationDelay;
    [SerializeField] float shootCooldown;
    [SerializeField] float despawnTime;
    GameObject previousBullet;
    bool fire;
    GameObject currentTrail;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag(targetTag).transform;
        targetCollider = target.GetComponentInParent<PlayerController>().GetComponent<Collider>();
        pointer = Instantiate(new GameObject("pointer"),Shootpoint).transform;
        pointer.position = Shootpoint.position;
        StartCoroutine(ShootCooldown());
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
        }
        if(state == behaviours.aim)
        {
            AimAtPlayer();
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
        if(mode == bulletType.projectile)
        {
            GameObject firedBullet = Instantiate(bullet, pointer.position, pointer.rotation);
            previousBullet = firedBullet;
            Physics.IgnoreCollision(firedBullet.GetComponent<Collider>(), transform.GetComponent<Collider>(), true);
            //Invoke("DestroyPrevious", despawnTime);
            fire = false;
        }
        else
        {
            Vector3 aim = new Vector3(Random.Range(-aimVariation.x, aimVariation.x), Random.Range(-aimVariation.y, aimVariation.y), Random.Range(-aimVariation.z, aimVariation.z)) + pointer.forward;          
            bool hit = Physics.Raycast(Shootpoint.position, aim.normalized, out RaycastHit ray, WhatBlocksMyView);
            Debug.DrawRay(Shootpoint.position, aim.normalized * shootDistance, Color.red);
            if (hit)
            {
                //print(ray.collider.name);
                if (ray.collider == targetCollider && targetCollider.TryGetComponent(out HealthManager healthManager))
                {
                    healthManager.HealthChange(-hitscanDamage);
                    if (mode != bulletType.melee && currentTrail == null)
                    {
                        StartCoroutine(SpawnTrail(ray));
                    }
                }
            }
        }

    }

    private IEnumerator SpawnTrail(RaycastHit rc)
    {
        currentTrail = Instantiate(bullet, Shootpoint.position,Shootpoint.rotation);
        float travelTime = currentTrail.GetComponent<TrailRenderer>().time;
        float currentTime = Time.time;
        while(Time.time < currentTime + travelTime)
        {
            float lerpTime = (Time.time - currentTime) / travelTime;
            currentTrail.transform.position = Vector3.Lerp(currentTrail.transform.position, rc.point, lerpTime);
            Debug.DrawLine(currentTrail.transform.position, rc.point, Color.black);
            yield return new WaitForEndOfFrame();
        }
        currentTrail.transform.position = rc.point;
        currentTrail.transform.GetChild(0).gameObject.SetActive(true);
        Destroy(currentTrail, despawnTime);
        currentTrail = null;
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
        if (state == behaviours.aim && !isRagdolling())
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


