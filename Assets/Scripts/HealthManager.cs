using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public float maxHealth;
    [Header("What Happens Upon Death?")]
    [SerializeField] MonoBehaviour deathScript;
    [SerializeField] string deathFunction;
    [Header("Regen")]
    [SerializeField] bool hasRegen;
    [SerializeField] float regenSpeed;
    [SerializeField] float regenTimer;
    float startRegenTime;

    [HideInInspector]public float health;
    [HideInInspector] public bool canTakeDamage;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        canTakeDamage = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasRegen)
        {
            Regen();
        }
    }
    public void HealthChange(float amount)
    {
        if (canTakeDamage)
        {
            if (amount < 0)
            {
                startRegenTime = Time.time + regenTimer;
            }
            if (health != 0)
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
    }

    void Regen()
    {
        if(Time.time >= startRegenTime && health < maxHealth)
        {
            health += Time.deltaTime * regenSpeed;
        }       
    }

    public void Death()
    {
        HealthChange(-health);
    }


}
