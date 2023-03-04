using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    float health;
    enum type {health,energy}
    [SerializeField] type barType;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if(barType == type.health)
        {
            health = PlayerController.playerHealth.health;
        }
        else
        {
            health = PlayerController.energy;
        }
        
        transform.localScale = new Vector3(Mathf.Lerp(transform.localScale.x,health / PlayerController.playerHealth.maxHealth,Time.deltaTime*3), 1, 1);
    }
}
