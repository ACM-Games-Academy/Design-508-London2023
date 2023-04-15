using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitscan : Shooter
{
    [HideInInspector] public GameObject currentTrail;
    [SerializeField] float hitscanDamage;
    [SerializeField] GameObject trail;

    public override void Shoot()
    {
        Vector3 aim = new Vector3(Random.Range(-aimVariation.x, aimVariation.x), Random.Range(-aimVariation.y, aimVariation.y), Random.Range(-aimVariation.z, aimVariation.z)) + pointer.forward;
        bool hit = Physics.Raycast(Shootpoint.position, aim.normalized, out RaycastHit ray, WhatBlocksMyView);
        Debug.DrawRay(Shootpoint.position, aim.normalized * shootDistance, Color.red);
        if (hit)
        {
            whenHit(ray);
        }
    }

    public virtual void whenHit(RaycastHit ray)
    {
        if (ray.collider == targetCollider && targetCollider.TryGetComponent(out HealthManager healthManager))
        {
            healthManager.HealthChange(-hitscanDamage);
            if (currentTrail == null && trail != null)
            {
                StartCoroutine(SpawnTrail(ray));
            }
        }
    }

    public IEnumerator SpawnTrail(RaycastHit rc)
    {
        currentTrail = Instantiate(trail, Shootpoint.position, Shootpoint.rotation);
        float travelTime = currentTrail.GetComponent<TrailRenderer>().time;
        float currentTime = Time.time;
        while (Time.time < currentTime + travelTime)
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
}
