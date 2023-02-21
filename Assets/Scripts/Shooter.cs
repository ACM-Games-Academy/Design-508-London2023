using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public enum behaviours{search,aim};
    public behaviours state;
    [SerializeField] string targetTag;
    Transform target;
    Transform pointer;
    [Header("Detecting The Target")]
    [SerializeField] LayerMask WhatBlocksMyView;
    [SerializeField] [Range(1,100)]float detectionDistance;
    [Header("Aiming at the Target")]
    [SerializeField] bool isStatic;
    [SerializeField] bool rotateAtTarget;
    [SerializeField] Transform Xrotater;
    [SerializeField] Transform Yrotater;
    [SerializeField] float Xoffset;
    [SerializeField] float Yoffset; 
    [Header("Shooting at the Target")]
    [SerializeField] Transform Shootpoint;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootCooldown;
    [SerializeField] float despawnTime;
    GameObject previousBullet;
    bool fire;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag(targetTag).transform;
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
        if(state == behaviours.search)
        {
            //look around animation
        }
        if(state == behaviours.aim)
        {
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
        bool hit = Physics.Raycast(pointer.position, pointer.forward, out RaycastHit ray, detectionDistance, WhatBlocksMyView);
        if (hit)
        {
            if(ray.collider.gameObject == target.gameObject && state != behaviours.aim)
            {
                state = behaviours.aim;
            }
            else if(ray.collider.gameObject != target.gameObject)
            {
                state = behaviours.search;
            }
        }
        else
        {
            state = behaviours.search;
        }
        Debug.DrawRay(Shootpoint.position, pointer.forward*detectionDistance, Color.blue);
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
        GameObject firedBullet = Instantiate(bullet, pointer.position, pointer.rotation);
        previousBullet = firedBullet;
        Physics.IgnoreCollision(firedBullet.GetComponent<Collider>(), transform.GetComponent<Collider>(), true);
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
        yield return new WaitForSeconds(shootCooldown);
        fire = true;
        yield return new WaitForEndOfFrame();
        StartCoroutine(ShootCooldown());
    }
}
