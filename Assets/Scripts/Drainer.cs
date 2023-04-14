using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drainer : Shooter
{
    [SerializeField] float energyLoss;
    public override void Shoot()
    {
        PlayerController.energy -= energyLoss;
    }
}
