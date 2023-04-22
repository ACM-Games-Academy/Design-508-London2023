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
    
    [Header("Spawning")]
    List<GameObject> currentVanLoad = new List<GameObject>();
    [SerializeField] GameObject swatVan;
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
    [HideInInspector] public List<GameObject> spawnedEnemies;
    int spawnedCount;



    bool fullySpawned;
    bool finished;
    float waitEndTime;
    GameObject waveUI;
    GameObject incomingUI;
    Slider waveBar;
    GameObject barUI;

    
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
        incomingUI = GameObject.FindGameObjectWithTag("incomingWaveUI");
        waveBar = waveUI.GetComponentInChildren<Slider>();
        barUI = waveBar.transform.parent.gameObject;
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
                    else if(spawnedCount != enemiesToSpawn.Length && !fullySpawned)
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
                if(GetKilledEnemies() >= spawnedCount)
                {
                    finished = true;
                    barUI.SetActive(false);
                    incomingUI.SetActive(true);
                    ClearOldEnemies();
                    if(waveNumber < waves.Length)
                    {
                        StartWait(timeBetweenWaves);
                    }                
                }
                break;
        }
    }

    void BeginWaves()
    {
        doorBlockers.SetActive(true);
        waveUI.SetActive(true);
        StartNextWave();       
    }

    void EndOfWaves()
    {
        state = waveStates.STOPPED;
        doorBlockers.SetActive(false);
        waveUI.SetActive(false);
    }

    void StartNextWave()
    {       
        finished = false;
        fullySpawned = false;
        currentWave = waves[waveNumber];
        waveNumber += 1;
        enemiesToSpawn = currentWave.enemies.ToArray();
        barUI.SetActive(true);
        UpdateCounterText();
        incomingUI.SetActive(false);
        waveBar.maxValue = enemiesToSpawn.Length;
        waveBar.value = waveBar.maxValue;
        spawnedEnemies = new List<GameObject>();
        spawnedCount = 0;
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
        waveBar.value = spawnedCount - GetKilledEnemies();
    }

    void UpdateCounterText()
    {
        waveNumText.text = "Wave " + waveNumber + "/" + waves.Length;
    }

    void ClearOldEnemies()
    {
        foreach(GameObject gob in spawnedEnemies)
        {
            Destroy(gob, 5f);
        }
    }

    void SpawnNextEnemy()
    {       
        Enemy nextEnemy = enemiesToSpawn[spawnedCount].GetComponent<Enemy>();
        Transform spawnPoint = GetSpawnPoint();
        if (nextEnemy.spawnPoint == Enemy.location.inVan)
        {
            currentVanLoad.Add(nextEnemy.gameObject);
            bool spawnVan = false;
            if(spawnedCount + 1 == enemiesToSpawn.Length)
            {
                spawnVan = true;
            }
            else
            {
                spawnVan = (enemiesToSpawn[spawnedCount+1].GetComponent<Enemy>().spawnPoint != Enemy.location.inVan);//if the enemy after this one doesn't spawn in a van
            }
            if (spawnVan)
            {
                GameObject van = Instantiate(swatVan, spawnPoint.position, spawnPoint.rotation, transform);//send the van with currently loaded enemies
                van.GetComponent<SwatVan>().troops = currentVanLoad;
                van.GetComponent<SwatVan>().currentWaveManager = this;
                currentVanLoad = new List<GameObject>();
            }
            spawnedCount += 1;
            StartWait(0);
        }
        else
        {
            GameObject newEnemy = Instantiate(nextEnemy.gameObject, spawnPoint.position, spawnPoint.rotation, transform);
            spawnedEnemies.Add(newEnemy);
            spawnedCount += 1;
            StartWait(spawnDelay);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && waveNumber < 1)
        {
            BeginWaves();
            foreach(Collider c in GetComponents<Collider>())
            {
                Destroy(c);
            }
        }
    }
}
