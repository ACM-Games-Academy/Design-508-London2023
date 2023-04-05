using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Wave))]
public class WaveManager : MonoBehaviour
{
    [SerializeField] float spawnDelay;
    [SerializeField] float timeBetweenWaves;

    enum waveStates
    {
        SPAWNING,
        SPAWNED,
        WAITING,
    }

    waveStates state;
    Wave[] waves;
    Wave currentWave;
    int waveNumber;
    GameObject[] enemiesToSpawn = new GameObject[0];
    List<Transform> spawnPoints = new List<Transform>();
    List<GameObject> spawnedEnemies;
    bool fullySpawned;
    bool finished;
    float waitEndTime;
    
    void Awake()
    {
        //get all the wave scripts that were added
        waves = GetComponents<Wave>();

        //add all the spawn points to a list variable
        foreach(Transform child in transform.GetComponentsInChildren<Transform>())
        {
            if(child.tag == "waveSpawn")
            {
                spawnPoints.Add(child);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {

        StartNextWave();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case waveStates.SPAWNING:
                SpawnNextEnemy();
                break;
            case waveStates.WAITING:
                if(Time.time >= waitEndTime)
                {
                    if (finished)
                    {
                        transform.SetParent(null);
                        StartNextWave();
                    }
                    else if(spawnedEnemies.Count != enemiesToSpawn.Length && !fullySpawned)
                    {
                        state = waveStates.SPAWNING;
                    }
                    else
                    {
                        fullySpawned = true;
                        state = waveStates.SPAWNED;                        
                    }
                }
                break;
            case waveStates.SPAWNED:
                if(GetKilledEnemies() >= spawnedEnemies.Count)
                {
                    finished = true;
                    ClearOldEnemies();
                    StartWait(timeBetweenWaves);
                }
                break;
        }
    }

    void StartNextWave()
    {
        finished = false;
        fullySpawned = false;
        currentWave = waves[waveNumber];
        waveNumber += 1;
        enemiesToSpawn = currentWave.enemies.ToArray();
        spawnedEnemies = new List<GameObject>();
        state = waveStates.SPAWNING;
    }

    Transform GetSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }

    void StartWait(float waitLength)
    {
        waitEndTime = Time.time + waitLength;
        state = waveStates.WAITING;
    }

    int GetKilledEnemies()
    {
        int number = 0;
        foreach(GameObject enemy in spawnedEnemies)
        {
            if (enemy.TryGetComponent(out HealthManager hm))
            {
                if (hm.dead)
                {
                    number += 1;
                }
            }
            else
            {
                number += 1;
                Debug.LogError("There are GameObjects dragged into a Wave Script with no Health Manager. GameObjects must have a Health Manager script to register as an Enemy");
            }
        }
        return number;
    }

    void ClearOldEnemies()
    {
        foreach(GameObject gob in spawnedEnemies)
        {
            Destroy(gob,5f);
        }
    }

    void SpawnNextEnemy()
    {
        Transform spawnPoint = GetSpawnPoint();
        GameObject newEnemy = Instantiate(enemiesToSpawn[spawnedEnemies.Count], spawnPoint.position, spawnPoint.rotation, transform);
        spawnedEnemies.Add(newEnemy);
        StartWait(spawnDelay);
    }

}
