using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwatVan : Enemy
{

    [Header("Van")]
    public List<GameObject> troops;
    [SerializeField] Transform troopSpawnPoint;
    [SerializeField] float troopSpawnDelay;
    public override bool PlayerInRange()
    {
        if (tooClose())
        {

        }
        return base.PlayerInRange(); 
    }

    void UnloadTroops()
    {

    }

}
