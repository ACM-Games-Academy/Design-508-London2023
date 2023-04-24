using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class fadeOut : MonoBehaviour
{
    bool fade;
    Image fadeScreen;
    TextMeshProUGUI endText;
    // Start is called before the first frame update
    void Start()
    {
        fadeScreen = GameManager.instance.fadeScreen;
        endText = fadeScreen.GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fade)
        {
            fadeScreen.color = Color.Lerp(GameManager.instance.fadeScreen.color, Color.black, Time.deltaTime);
            endText.color = Color.Lerp(endText.color, Color.white,Time.deltaTime*0.8f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            fade = true;
            Invoke("LoadMenu", 5f);
        }
    }


    void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }
}
