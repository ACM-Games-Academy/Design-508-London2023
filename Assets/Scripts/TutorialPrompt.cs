using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialPrompt : MonoBehaviour
{
    [Header("Tags")]
    [SerializeField] string setOffTag = "Player";
    [SerializeField] string uiTextTag = "tutorialText";
    [Header("Type tutorial message here:")]
    [SerializeField] string tutorialMessage;
    TextMeshProUGUI tutorialText;
    

    // Start is called before the first frame update
    void Start()
    {
        tutorialText = GameObject.FindGameObjectWithTag("tutorialText").GetComponent<TextMeshProUGUI>();
        tutorialText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == setOffTag)
        {
            tutorialText.gameObject.SetActive(true);
            tutorialText.text = tutorialMessage;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == setOffTag)
        {
            tutorialText.gameObject.SetActive(false);
            tutorialText.text = null;
        }
    }
}
