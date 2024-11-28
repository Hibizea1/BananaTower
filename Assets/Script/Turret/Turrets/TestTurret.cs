#region

using System;
using System.Collections;

#endregion

public class TestTurret : Turret, ITurretEffect
{
    IEnumerator ITurretEffect.ApplyEffect()
    {
        throw new NotImplementedException();
    }

    protected override void Shoot()
    {
    }

    public override void Upgrade()
    {
    }
}