using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public enum behaviours{search,aim};
    public behaviours state;
    [SerializeField] string targetTag;
    Collider targetCollider;
    Transform target;
    Transform pointer;
    [Header("Detecting The Target")]
    [SerializeField] LayerMask WhatBlocksMyView;
    [SerializeField] [Range(1,100)]float shootDistance;
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
    [SerializeField] float shootCooldown;
    [SerializeField] float despawnTime;
    GameObject previousBullet;
    bool fire;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag(targetTag).transform;
        targetCollider = target.GetComponentInParent<PlayerController>().GetComponent<Collider>();
        pointer = Instantiate(new GameObject("pointer")).transform;
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
            print("search");
        }
        if(state == behaviours.aim)
        {
            if(TryGetComponent(out Ragdoll rs))
            {
                fire = !rs.ragdoll;
            }
            print("fire");
            AimAtPlayer();
            if (fire)
            {
                Shoot();
            }
        }
        

    }


    void Detection()
    {
        pointer.LookAt(target);
        bool hit = Physics.Raycast(pointer.position, pointer.forward, out RaycastHit ray, shootDistance, WhatBlocksMyView);
        Debug.DrawRay(pointer.position, pointer.forward * shootDistance, Color.red);
        if (hit)
        {
            print(ray.collider.name);
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
        if (rotateAtTarget)
        {
            float XAngle = 180 + Mathf.Atan2(Xrotater.position.y - target.position.y, Xrotater.position.z + Mathf.Abs(target.position.z)) * Mathf.Rad2Deg + Xoffset;//Find X angle toward target
            Xrotater.localRotation = Quaternion.Euler(XAngle, 0, 0);//Quaternion.Euler(Mathf.LerpAngle(Xrotater.localRotation.x, XAngle, rotationSpeed * Time.deltaTime), 0, 0);//Lerp rotation to target X angle

            float YAngle = 180 + Mathf.Atan2(Yrotater.position.x - target.position.x, Yrotater.position.z - target.position.z) * Mathf.Rad2Deg + Yoffset;//Find Y angle toward target
            Yrotater.localRotation = Quaternion.Euler(0, YAngle, 0);//Quaternion.Euler(0,Mathf.LerpAngle(Yrotater.localRotation.x, YAngle, rotationSpeed * Time.deltaTime), 0);//Lerp rotation to target Y angle
        }
    }

    void Shoot()
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
            bool hit = Physics.Raycast(pointer.position, aim.normalized, out RaycastHit ray, WhatBlocksMyView);
            if (hit)
            {
                if (mode != bulletType.melee)
                {
                    StartCoroutine(SpawnTrail(Shootpoint, ray));
                }            
                if (ray.collider == targetCollider && targetCollider.TryGetComponent(out HealthManager healthManager))
                {
                    healthManager.HealthChange(-hitscanDamage);
                }
            }
        }

    }

    private IEnumerator SpawnTrail(Transform place, RaycastHit rc)
    {
        GameObject trail = Instantiate(bullet, place.position, place.rotation);
        float travelTime = trail.GetComponent<TrailRenderer>().time;
        float currentTime = Time.time;
        while(Time.time < currentTime + travelTime)
        {
            float lerpTime = (Time.time - currentTime) / travelTime;
            trail.transform.position = Vector3.Lerp(trail.transform.position, rc.point, lerpTime);
            yield return new WaitForEndOfFrame();
        }
        trail.transform.position = rc.point;
        trail.transform.GetChild(0).gameObject.SetActive(true);
        Destroy(trail, despawnTime);
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
        yield return new WaitForSeconds(shootCooldown);
        fire = true;
        yield return new WaitForEndOfFrame();
        StartCoroutine(ShootCooldown());
    }
}
