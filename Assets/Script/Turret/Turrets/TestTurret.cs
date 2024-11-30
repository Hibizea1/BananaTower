using System;
using UnityEngine;

public class TestTurret : Turret
{



    
    
    
    
    
    
    [ContextMenu("TestUpgrade")]
    public override void Upgrade()
    {
        Damage += 1;
    }
}