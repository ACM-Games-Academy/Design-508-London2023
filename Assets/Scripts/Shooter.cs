using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public enum behaviours{search,aim,destroy};
    [SerializeField] behaviours state;
    [SerializeField] string targetTag;
    Transform target;
    Transform pointer;
    [Header("Detecting The Target")]
    [SerializeField] LayerMask WhatBlocksMyView;
    [SerializeField] [Range(1,100)]float detectionDistance;
    [Header("Aiming at the Target")]
    [SerializeField] Transform Xrotater;
    [SerializeField] float Xoffset;
    [SerializeField] float Yoffset;
    [SerializeField] Transform Yrotater;
    [Header("Shooting at the Target")]
    [SerializeField] Transform Shootpoint;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootCooldown;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag(targetTag).transform;
<<<<<<< Updated upstream
=======
        targetCollider = target.GetComponentInParent<PlayerController>().GetComponent<Collider>();

>>>>>>> Stashed changes
        pointer = Instantiate(new GameObject("pointer")).transform;
        pointer.position = Shootpoint.position;
    }

    // Update is called once per frame
    void Update()
    {
        Detection();
        if(state == behaviours.search)
        {
            //look around animation
        }
        else if(state == behaviours.aim)
        {
            AimAtPlayer();
        }
        else if(state == behaviours.destroy)
        {
            AimAtPlayer();
            Shoot();
            StartAttackCycle();
        }
    }


    void Detection()
    {
        pointer.LookAt(target);
        bool hit = Physics.Raycast(pointer.position, pointer.forward, out RaycastHit ray, detectionDistance, WhatBlocksMyView);
        if (hit)
        {
            if(ray.collider.gameObject == target.gameObject && state == behaviours.search)
            {
                StartAttackCycle();
            }
            else if(ray.collider.gameObject != target.gameObject)
            {
                state = behaviours.search;
                StopCoroutine(ShootCooldown());
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
        float XAngle = 180 + Mathf.Atan2(Xrotater.position.y - target.position.y, Xrotater.position.z + Mathf.Abs(target.position.z)) * Mathf.Rad2Deg + Xoffset;//Find X angle toward target
        Xrotater.localRotation = Quaternion.Euler(XAngle, 0, 0);//Quaternion.Euler(Mathf.LerpAngle(Xrotater.localRotation.x, XAngle, rotationSpeed * Time.deltaTime), 0, 0);//Lerp rotation to target X angle

        float YAngle = 180 + Mathf.Atan2(Yrotater.position.x - target.position.x, Yrotater.position.z - target.position.z) * Mathf.Rad2Deg + Yoffset;//Find Y angle toward target
        Yrotater.localRotation = Quaternion.Euler(0,YAngle, 0);//Quaternion.Euler(0,Mathf.LerpAngle(Yrotater.localRotation.x, YAngle, rotationSpeed * Time.deltaTime), 0);//Lerp rotation to target Y angle
    }

    void StartAttackCycle()
    {
             state = behaviours.aim;
            StartCoroutine(ShootCooldown());
    }

    void Shoot()
    {
        GameObject firedBullet = Instantiate(bullet, pointer.position, pointer.rotation);
        Physics.IgnoreCollision(firedBullet.GetComponent<Collider>(), transform.GetComponent<Collider>(), true);
    }

    IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(shootCooldown);
        state = behaviours.destroy;
    }
}
