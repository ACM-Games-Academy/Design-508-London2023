using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public float health;
<<<<<<< Updated upstream
    public float maxHealth;
    // Start is called before the first frame update
    void Start()
    {
        maxHealth = PlayerController.playerHealth.maxHealth;
=======
    //public float maxHealth;
    // Start is called before the first frame update
    void Start()
    {
        
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
        health = PlayerController.playerHealth.health;
<<<<<<< Updated upstream
        transform.localScale = new Vector3(health / maxHealth, 1, 1);
=======
>>>>>>> Stashed changes
    }
}
