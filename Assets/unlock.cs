using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unlock : MonoBehaviour
{
    public enum powers { speed , laser, flight};
    public powers powerUnlock;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            switch (powerUnlock)
            {
                case powers.speed:
                    GameManager.superSpeed = true;
                    break;
                case powers.laser:
                    GameManager.laserVision = true;
                    break;
                case powers.flight:
                    GameManager.flight = true;
                    break;
            }
            GameManager.player.LoadManagerVariables();
            Destroy(gameObject);
        }
        
    }
}
