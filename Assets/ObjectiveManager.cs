using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    public static Vector3 currentObjectiveLocation;
    public static string currentObjectiveText;
    public static TextMeshProUGUI objectiveUIText;
    public static GameObject marker;
    // Start is called before the first frame update
    void Awake()
    {
        objectiveUIText = GameObject.FindGameObjectWithTag("objectiveText").GetComponent<TextMeshProUGUI>();
        marker = GameObject.FindGameObjectWithTag("marker");
        currentObjectiveLocation = transform.position;
}

    // Update is called once per frame
    void Update()
    {
        marker.transform.position = Camera.main.WorldToScreenPoint(currentObjectiveLocation);
    }

    public static void UpdateObjective()
    {
        objectiveUIText.text = currentObjectiveText;
        
    }
}
