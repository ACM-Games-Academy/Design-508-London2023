using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drainer : Hitscan
{
    [SerializeField] float energyLoss;

    public override void whenHit(RaycastHit ray)
    {
        PlayerController.energy -= energyLoss;
        if (currentTrail == null)
        {
            StartCoroutine(SpawnTrail(ray));
        }
    }
}
