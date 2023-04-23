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


        transform.SetParent(Camera.main.transform);
        transform.localPosition = Vector3.zero;
}

    // Update is called once per frame
    void Update()
    {
        if (FacingObjective() && currentObjectiveLocation != null)
        {
            marker.SetActive(true);
            marker.transform.position = Camera.main.WorldToScreenPoint(currentObjectiveLocation);
        }
        else
        {
            marker.SetActive(false);
        }
    }

    public static void UpdateObjective()
    {
        objectiveUIText.text = currentObjectiveText;
        
    }

    bool FacingObjective()
    {
        transform.LookAt(currentObjectiveLocation);
        Debug.DrawRay(transform.position, transform.forward,Color.red);

        return Vector3.Angle(transform.forward, Camera.main.transform.forward) < Camera.main.fieldOfView;
    }
}
