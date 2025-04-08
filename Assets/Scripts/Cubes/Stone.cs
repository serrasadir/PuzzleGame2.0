using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : ObstacleCube
{
    public override bool AffectedByGravity => false;
    public override bool AffectedByBlast => false;
    protected override void OnTap()
    {
        base.OnTap();

    }

    public void TakeTNTDamage()
    {
        TakeDamage();
        // Only takes damage from TNT, so no immediate damage here
        //BURADAKİ MANTIĞA GERİ DÖNELİM UNUTMA
    }
}
