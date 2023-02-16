using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public float health;
    [SerializeField] float maxHealth;
    [Header("Death Functionality")]
    [SerializeField] string dieFunction;
    [SerializeField] GameObject dieFunctionSource;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void HealthChange(float amount)
    {
        health -= amount;
        if(health < 0)
        {
            health = 0;
        }
        else if(health > maxHealth)
        {
            health = maxHealth;
        }
    }
}
