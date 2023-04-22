using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class elipses : MonoBehaviour
{
    [SerializeField] float delay;
    TextMeshProUGUI text;
    string firstPart;
    // Start is called before the first frame update
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        Cycle();
    }

    string[] ElipsesSplit()
    {
        return text.text.Split('.');
    }

    void Cycle()
    {
        if(ElipsesSplit().Length < 4)
        {
            firstPart += '.';
        }
        else
        {
            firstPart = ElipsesSplit()[0];
        }
        text.text = firstPart;
        Invoke("Cycle", delay);
    }
}
