#region

using System.Collections.Generic;
using UnityEngine;

#endregion

[RequireComponent(typeof(CircleCollider2D))]
public abstract class Turret : MonoBehaviour
{
    [SerializeField] int _damage;
    [SerializeField] int _range;
    [SerializeField] float _shootRate;
    [SerializeField] int _magazineSize;
    [SerializeField] float _reloadTime;

    int _currentMagazine;

    CircleCollider2D _detectionCollider;

    protected List<GameObject> EnemiesInRange; //set to enemy class for optimisation

    float _reloadTimer;
    float _shootTimer;

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

    public float ShootRate
    {
        get => _shootRate;
        protected set => _shootRate = value;
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

    void Start()
    {
        EnemiesInRange = new List<GameObject>();
        _detectionCollider = GetComponent<CircleCollider2D>();

        _currentMagazine = MagazineSize;
        _reloadTimer = 0;
        _shootTimer = 0;

        _detectionCollider.radius = Range;
    }

    void Update()
    {
        CheckOnReload();

        CheckOnShoot();
    }

    void OnCollisionEnter(Collision other)
    {
        //if (other.gameObject.TryGetComponent()) Component for enemy
        {
            EnemiesInRange.Add(other.gameObject); //add the component
        }
    }

    void OnCollisionExit(Collision other)
    {
        //if (other.gameObject.TryGetComponent()) Component for enemy
        {
            EnemiesInRange.Remove(other.gameObject); //add the component
        }
    }

    public void LoadData(int damage, int range, float shootRate, int magazineSize, float reloadTime, string loadName)
    {
        Damage = damage;
        Range = range;
        ShootRate = shootRate;
        MagazineSize = magazineSize;
        ReloadTime = reloadTime;
        name = loadName;
    }

    protected abstract void Shoot();
    public abstract void Upgrade();

    protected virtual void Reload()
    {
        _currentMagazine = MagazineSize;
    }

    void CheckOnReload()
    {
        if (_currentMagazine <= 0)
        {
            _reloadTimer += Time.deltaTime;

            if (_reloadTimer >= ReloadTime)
            {
                Reload();
                _reloadTimer = ReloadTime;
            }
        }
    }

    void CheckOnShoot()
    {
        if (EnemiesInRange.Count > 0 && _currentMagazine > 0)
        {
            _shootTimer += Time.deltaTime;
            if (_shootTimer >= ShootRate) Shoot();
        }
    }
}