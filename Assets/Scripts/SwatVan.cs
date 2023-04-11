using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwatVan : Enemy
{

    [Header("Van")]
    public List<GameObject> troops;
    public override bool PlayerInRange()
    {
        return base.PlayerInRange(); 
    }

}
