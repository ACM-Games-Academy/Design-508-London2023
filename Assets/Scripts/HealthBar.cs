using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    float health;
    float maxHealth;
    // Start is called before the first frame update
    void Start()
    {
        maxHealth = PlayerController.playerHealth.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        health = PlayerController.playerHealth.health;

        transform.localScale = new Vector3(health / maxHealth, 1, 1);
    }
}
