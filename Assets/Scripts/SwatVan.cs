using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwatVan : Enemy
{
    [Header("Van")]
    public List<GameObject> troops;
    [HideInInspector]public WaveManager currentWaveManager;
    [SerializeField] Transform troopSpawnPoint;
    [SerializeField] float troopSpawnDelay;
    
    bool deployed;
    public override void Awake()
    {
        base.Awake();
        deployed = false;
    }

    public override bool PlayerInRange()
    {
        //this function basically defines whether or not the enemy moves toward the player, usually based on range
        if (!deployed)
        {
            if (tooClose())
            {
                StartCoroutine(DeployTroops());
                return false;
            }
            else
            {
                return base.PlayerInRange();
            }
        }
        else
        {
            return false;
        }

    }

    IEnumerator DeployTroops()
    {
        deployed = true;
        ToggleThrowable(true);
        foreach (GameObject troop in troops)
        {
            yield return new WaitForSeconds(troopSpawnDelay);
            if(troop.TryGetComponent(out Enemy enemyScript))
            {
                GameObject enemy = Instantiate(troop, troopSpawnPoint.position, troopSpawnPoint.rotation);
                if(TryGetComponent(out Throwable ts) && enemy.TryGetComponent(out Ragdoll enemyRs))
                {
                    if(ts.beingHeld || ts.beenThrown)
                    {
                        enemyRs.Invoke("StartRagdoll",0.1f);
                    }
                }
                if(currentWaveManager != null)
                {
                    currentWaveManager.spawnedEnemies.Add(enemy);
                }
            }
        }
        troops = new List<GameObject>();
    }

    public override void Die()
    {
        troopSpawnDelay = 0;
        if (!deployed)
        {
            StartCoroutine(DeployTroops());
        }

        GetComponent<Explodable>().Invoke("Explode", 0.3f);
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
    }

    private void OnDisable()
    {
        ToggleThrowable(true);
    }

    private void OnEnable()
    {
        ToggleThrowable(false);
    }

}
