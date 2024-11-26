using System.Collections;

public class TestTurret : Turret, ITurretEffect
{
    protected override void Shoot()
    {
    }

    public override void Upgrade()
    {
    }

    IEnumerator ITurretEffect.ApplyEffect()
    {
        throw new System.NotImplementedException();
    }
}