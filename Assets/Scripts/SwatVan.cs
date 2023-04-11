using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwatVan : Enemy
{

    [Header("Van")]
    public List<GameObject> troops;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool PlayerInRange()
    {
        return base.PlayerInRange(); 
    }
}
