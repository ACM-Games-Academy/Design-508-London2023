using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VictoryZone : MonoBehaviour
{
    [SerializeField] int playersTouching;
    List<GameObject> playersOnMe = new List<GameObject>(0);
    [SerializeField] GameObject WinScreen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
     if(playersTouching == GameManager.instance.playerNumber)
        {
            WinScreen.SetActive(true);
            GameManager.instance.won = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject colliderObject = other.gameObject;
        if (colliderObject.tag == "Player")
        {
            //if the player object isn't already on the victory zone
            if (!playersOnMe.Contains(colliderObject))
            {
                playersOnMe.Add(colliderObject);
                playersTouching = playersOnMe.Count;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject colliderObject = other.gameObject;
        if (colliderObject.tag == "Player")
        {
            //if this object is in the list of touching objects then remove it
            if (playersOnMe.Contains(colliderObject))
            {
                playersOnMe.Remove(colliderObject);
                playersTouching = playersOnMe.Count;
            }
        }
    }
}
