using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : ObstacleCube
{
    protected override void OnTap()
    {
        base.OnTap();
        TakeDamage();
    }
}
