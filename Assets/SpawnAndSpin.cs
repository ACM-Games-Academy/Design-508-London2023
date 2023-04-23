using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAndSpin : MonoBehaviour
{
    [SerializeField] GameObject level;
    public float t;
    public Transform roticeryObject;
    // Start is called before the first frame update
    void Start()
    {
        if(level != null)
        {
            Instantiate(level, transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, roticeryObject.rotation, t * Time.deltaTime);
    }
}
