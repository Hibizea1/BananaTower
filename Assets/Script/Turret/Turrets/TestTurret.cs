using System;
using UnityEngine;

public class TestTurret : Turret
{
    protected override void Start()
    {
        ReloadSlider.maxValue = _reloadTime;
    }

    protected override void Shoot()
    {
        _enemiesInRange[0].TakeDamage(_damage);
        EventMaster.GetInstance().InvokeEvent("CheckCount");
        Debug.Log("Bang");
    }

    [ContextMenu("TestUpgrade")]
    public override void Upgrade()
    {
        if (MoneyManager.GetInstance().CheckMoneyCount(UpgradeCost))
        {
            LevelCount++;
            Damage += IncresedDamagePerUpgrade;
            _range += IncresedRangePerUpgrade;
            _reloadTime -= DecreseReloadTimePerUpgrade;
            if (_reloadTime <= 1)
            {
                _reloadTime = 1;
            }
            ReloadSlider.maxValue = _reloadTime;
            EventMaster.GetInstance().InvokeEventInt("RemoveMoney", UpgradeCost);
            UpgradeCost *= 2;
            float redValue = Mathf.Clamp01(LevelCount * 0.1f);
            _spriteRenderer.color = new Color(1f, 1f - redValue, 1f - redValue);
        }
        else
        {
            TextScroller.GetInstance().LaunchTextScroll();
        }
    }

    protected override void CheckOnShoot()
    {
        if (_enemiesInRange.Count > 0 && _currentMagazine > 0)
        {
            _shootTimer += Time.deltaTime;
            if (_shootTimer >= TimeToShoot)
            {
                Shoot();
                _currentMagazine--;
                Debug.Log(_currentMagazine);
                _shootTimer = 0.0f;
            }
        }
    }
}