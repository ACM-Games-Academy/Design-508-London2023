using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwatVan : Enemy
{

    [Header("Van")]
    public List<GameObject> troops;
    [SerializeField] Transform troopSpawnPoint;
    [SerializeField] float troopSpawnDelay;
    bool deployed;
    public override void Awake()
    {
        base.Awake();
        deployed = false;
        ToggleThrowable(false);
    }

    public override bool PlayerInRange()
    {
        if (tooClose() && !deployed)
        {
            DeployTroops();
        }
        return base.PlayerInRange(); 
    }

    IEnumerator DeployTroops()
    {
        agent.enabled = false;
        foreach(GameObject troop in troops)
        {
            yield return new WaitForSeconds(troopSpawnDelay);
            if(troop.TryGetComponent(out Enemy enemyScript))
            {
                Instantiate(troop, troopSpawnPoint.position, troopSpawnPoint.rotation);
            }
        }
        deployed = true;
        ToggleThrowable(true);
    }

    void ToggleThrowable(bool toggle)
    {
        if(TryGetComponent(out Throwable ts))
        {
            ts.enabled = toggle;
        }
        if(TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = !toggle;
        }
        agent.enabled = !toggle;
    }

}
