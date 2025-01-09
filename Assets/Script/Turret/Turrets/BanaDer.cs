using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BanaDer : Turret
{
    [SerializeField] int IncreasePower;
    [SerializeField] LayerMask LayerMask;
    [SerializeField] ParticleSystem ParticleSystem;


    bool _isActive = false;
    bool _buff = false;
    bool _nerf = false;

    protected override void Start()
    {
        ParticleSystem.Stop();
        EventMaster.GetInstance().GetEvent("ActiveBanaDer").AddListener(StartBuff);
    }

    protected override void Shoot()
    {
        if (_buff) return;

        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, _range, LayerMask);
        foreach (Collider2D collider2D1 in hit)
        {
            if (collider2D1 != GetComponent<BoxCollider2D>())
            {
                collider2D1.GetComponent<Turret>().Damage += IncreasePower;
                _buff = true;
            }
        }
    }

    public override void Upgrade()
    {
        throw new System.NotImplementedException();
    }

    protected override void CheckOnShoot()
    {
    }

    public void StartBuff()
    {
        if (!_isActive)
        {
            _isActive = true;
            StartCoroutine(ShootAndReload());
            ParticleSystem.Play();
        }
    }

    private IEnumerator ShootAndReload()
    {
        // Shoot for the duration of timeToShoot
        Shoot();
        yield return new WaitForSeconds(_timeToTimeToShoot);
        ParticleSystem.Stop();

        // Revert the shoot effect
        RevertShoot();

        // Reload for the duration of reloadTime
        ReloadSlider.gameObject.SetActive(true);
        ReloadSlider.maxValue = _reloadTime;
        ReloadSlider.value = 0;

        while (ReloadSlider.value < _reloadTime)
        {
            ReloadSlider.value += Time.deltaTime;
            yield return null;
        }

        // Reset flags
        ReloadSlider.gameObject.SetActive(false);
        _isActive = false;
        _buff = false;
        _nerf = false;
    }

    void RevertShoot()
    {
        if (_nerf) return;

        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, _range, LayerMask);
        foreach (Collider2D collider2D1 in hit)
        {
            if (collider2D1 != GetComponent<BoxCollider2D>())
            {
                collider2D1.GetComponent<Turret>().Damage -= IncreasePower;
                _nerf = true;
            }
        }
    }
}