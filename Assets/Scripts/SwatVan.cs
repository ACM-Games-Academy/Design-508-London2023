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
        ToggleThrowable(false);
    }

    public override bool PlayerInRange()
    {
        if (tooClose() && !deployed)
        {
            StartCoroutine(DeployTroops());
        }
        return base.PlayerInRange(); 
    }

    IEnumerator DeployTroops()
    {
        agent.enabled = false;
        deployed = true;
        foreach (GameObject troop in troops)
        {
            yield return new WaitForSeconds(troopSpawnDelay);
            if(troop.TryGetComponent(out Enemy enemyScript))
            {
                GameObject enemy = Instantiate(troop, troopSpawnPoint.position, troopSpawnPoint.rotation);
                if(currentWaveManager != null)
                {
                    currentWaveManager.spawnedEnemies.Add(enemy);
                }
            }
        }        
        this.enabled = false;
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

    private void OnDisable()
    {
        ToggleThrowable(true);
    }

    private void OnEnable()
    {
        ToggleThrowable(false);
    }

}
