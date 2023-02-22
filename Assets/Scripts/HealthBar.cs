using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    float health;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        health = PlayerController.playerHealth.health;
        transform.localScale = new Vector3(Mathf.Lerp(transform.localScale.x,health / PlayerController.playerHealth.maxHealth,Time.deltaTime), 1, 1);
    }
}
