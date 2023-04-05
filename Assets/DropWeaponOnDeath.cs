using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropWeaponOnDeath : MonoBehaviour
{
    [SerializeField] GameObject[] Dropables;
    

    // Start is called before the first frame update
    public void Drop()
    {
        foreach (GameObject dropable in Dropables)
        {
            Instantiate(dropable, transform.position, transform.rotation);
        }
    }
}
