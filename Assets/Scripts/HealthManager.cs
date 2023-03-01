using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public float maxHealth;
    [Header("What Happens Upon Death?")]
    [SerializeField] MonoBehaviour deathScript;
    [SerializeField] string deathFunction;
    [Header("Set Automatically by Script:")]
    public float health;
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
        if(health != 0)
        {
            health += amount;
            if (health <= 0)
            {
                health = 0;
                deathScript.Invoke(deathFunction, 0);
            }
            else if (health > maxHealth)
            {
                health = maxHealth;
            }
        }
    }

    public void Death()
    {
        HealthChange(-health);
    }
}
