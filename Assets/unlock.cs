using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class unlock : MonoBehaviour
{
    public enum powers { speed , laser, flight};
    public powers powerUnlock;
    GameObject tutorial;
    TextMeshProUGUI tutorialText;

    // Start is called before the first frame update
    void Start()
    {
        tutorialText = GameManager.instance.powerTutorial;
        tutorial = tutorialText.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            tutorial.SetActive(true);
            switch (powerUnlock)
            {
                case powers.speed:
                    GameManager.superSpeed = true;
                    tutorialText.text = "Double Press Sprint to activate Super Speed";
                    break;
                case powers.laser:
                    GameManager.laserVision = true;
                    tutorialText.text = "Hold LMB/Left Trigger to use Laser Vision!";
                    break;
                case powers.flight:
                    GameManager.flight = true;
                    tutorialText.text = "Press Jump while in mid-air to activate flight!";
                    break;
            }
            GameManager.player.LoadManagerVariables();
            Destroy(GetComponent<Collider>());
            Destroy(GetComponent<Renderer>());
            Destroy(gameObject, 5f);
        }
        
    }

    private void OnDestroy()
    {
        if(tutorial != null)
        {
            tutorial.SetActive(false);
        }
    }
}
