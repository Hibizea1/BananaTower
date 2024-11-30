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
        var enemiesClosest = EnemiesInRange[0];
        enemiesClosest.GetComponent<MonkeyBase>().TakeDamage(Damage);
    }

    public override void Upgrade()
    {
    }
}