using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(CircleCollider2D))]
public abstract class Turret : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private int _range;

    [FormerlySerializedAs("_timeToShoot")] [SerializeField]
    private float _timeToTimeToShoot;

    [SerializeField] private int _magazineSize;
    [SerializeField] private float _reloadTime;

    #region PropertySettings

    public int Damage
    {
        get => _damage;
        protected set => _damage = value;
    }

    public int Range
    {
        get => _range;
        protected set => _range = value;
    }

    public float TimeToShoot
    {
        get => _timeToTimeToShoot;
        protected set => _timeToTimeToShoot = value;
    }

    public int MagazineSize
    {
        get => _magazineSize;
        protected set => _magazineSize = value;
    }

    public float ReloadTime
    {
        get => _reloadTime;
        protected set => _reloadTime = value;
    }

    #endregion


    private int _currentMagazine;

    private float _reloadTimer;
    private float _shootTimer;

    private CircleCollider2D _detectionCollider;

    [SerializeField] private List<MonkeyBase> _enemiesInRange;

    private void Start()
    {
        _enemiesInRange = new List<MonkeyBase>();
        _detectionCollider = GetComponent<CircleCollider2D>();

        _currentMagazine = _magazineSize;
        _reloadTimer = 0;
        _shootTimer = 0;

        _detectionCollider.radius = _range;
    }

    private void Update()
    {
        CheckOnReload();

        CheckOnShoot();
    }

    protected virtual void Shoot()
    {
        _enemiesInRange[0].TakeDamage(_damage);
        //TODO : Instantiate projectile maybe deal damage with projectile
        Debug.Log("Bang");
    }

    public abstract void Upgrade();

    protected virtual void Reload()
    {
        _currentMagazine = MagazineSize;
    }

    private void CheckOnReload()
    {
        if (_currentMagazine <= 0)
        {
            _reloadTimer += Time.deltaTime;

            if (_reloadTimer >= ReloadTime)
            {
                Reload();
                _reloadTimer = 0.0f;
            }
        }
    }

    private void CheckOnShoot()
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


    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger");
        if (other.gameObject.TryGetComponent(out MonkeyBase c))
            _enemiesInRange.Add(c);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out MonkeyBase c))
            _enemiesInRange.Remove(c);
    }
}