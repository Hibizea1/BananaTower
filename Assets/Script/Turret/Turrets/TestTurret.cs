using System;
using UnityEngine;

public class TestTurret : Turret
{

    protected override void Shoot()
    {
        _enemiesInRange[0].TakeDamage(_damage);
        EventMaster.GetInstance().InvokeEvent("CheckCount");
        //TODO : Instantiate projectile maybe deal damage with projectile
        Debug.Log("Bang");
    }

    [ContextMenu("TestUpgrade")]
    public override void Upgrade()
    {
        Damage += 1;
    }

    protected override void CheckOnShoot()
    {
        if (_enemiesInRange.Count > 0 && _currentMagazine > 0)
        {
            _shootTimer += Time.deltaTime;
            if (_shootTimer >= TimeToShoot)
            {
                Shoot();
                _shootTimer = 0.0f;
            }
        }
    }
    
}