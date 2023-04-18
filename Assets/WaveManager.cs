using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Wave))]
public class WaveManager : MonoBehaviour
{
    [SerializeField] float spawnDelay;
    [SerializeField] float timeBetweenWaves;
    [SerializeField] GameObject doorBlockers;
    

    enum waveStates
    {
        SPAWNING,
        SPAWNED,
        WAITING,
        STOPPED,
    }

    waveStates state;
    Wave[] waves;
    Wave currentWave;
    int waveNumber;
    TextMeshProUGUI waveNumText;
    GameObject[] enemiesToSpawn = new GameObject[0];
    List<Transform> spawnPoints = new List<Transform>();
    List<GameObject> spawnedEnemies;
    bool fullySpawned;
    bool finished;
    float waitEndTime;
    GameObject waveUI;
    Slider waveBar;

    
    void Awake()
    {
        //get all the wave scripts that were added
        waves = GetComponents<Wave>();
        state = waveStates.STOPPED;
        //add all the spawn points to a list variable
        foreach(Transform child in transform.GetComponentsInChildren<Transform>())
        {
            if(child.tag == "waveSpawn")
            {
                spawnPoints.Add(child);
            }
        }

        //UI Assigning
        waveUI = GameObject.FindGameObjectWithTag("waveUI");
        waveNumText = GameObject.FindGameObjectWithTag("waveText").GetComponent<TextMeshProUGUI>();
        waveBar = waveUI.GetComponentInChildren<Slider>();
        waveUI.SetActive(false) ;
        
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
                        if (currentWave.triggerUpgrade)
                        {
                            //shakirs function for upgrading
                        }
                        else if(waveNumber < waves.Length)
                        {
                            StartNextWave();
                        }
                        else
                        {
                            EndOfWaves();
                        }
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
                UpdateUI();
                if(GetKilledEnemies() >= spawnedEnemies.Count)
                {
                    finished = true;
                    waveUI.SetActive(false);
                    ClearOldEnemies();
                    StartWait(timeBetweenWaves);
                }
                break;
        }
    }

    void BeginWaves()
    {
        doorBlockers.SetActive(true);
        StartNextWave();       
    }

    void EndOfWaves()
    {
        state = waveStates.STOPPED;
        doorBlockers.SetActive(false);
    }

    void StartNextWave()
    {       
        finished = false;
        fullySpawned = false;
        currentWave = waves[waveNumber];
        waveNumber += 1;
        enemiesToSpawn = currentWave.enemies.ToArray();
        waveUI.SetActive(true);
        UpdateCounterText();
        waveBar.maxValue = enemiesToSpawn.Length;
        waveBar.value = waveBar.maxValue;
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

    void UpdateUI()
    {
        waveBar.value = spawnedEnemies.Count - GetKilledEnemies();
    }

    void UpdateCounterText()
    {
        waveNumText.text = "Wave " + waveNumber + "/" + waves.Length;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && waveNumber < 1)
        {
            BeginWaves();
            foreach(Collider c in GetComponentsInChildren<Collider>())
            {
                Destroy(c);
            }
        }
    }
}
