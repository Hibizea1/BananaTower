using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public abstract class Turret : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private int _range;
    [SerializeField] private float _shootRate;
    [SerializeField] private int _magazineSize;
    [SerializeField] private float _reloadTime;

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

    private int _currentMagazine;

    private float _reloadTimer;
    private float _shootTimer;

    private CircleCollider2D _detectionCollider;

    private List<GameObject> _enemiesInRange; //set to enemy class for optimisation

    private void Start()
    {
        _enemiesInRange = new List<GameObject>();
        _detectionCollider = GetComponent<CircleCollider2D>();

        _currentMagazine = MagazineSize;
        _reloadTimer = 0;
        _shootTimer = 0;

        _detectionCollider.radius = Range;
    }

    private void Update()
    {
        CheckOnReload();

        CheckOnShoot();
    }

    protected abstract void Shoot();
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
                _reloadTimer = ReloadTime;
            }
        }
    }

    private void CheckOnShoot()
    {
        if (_enemiesInRange.Count > 0 && _currentMagazine > 0)
        {
            _shootTimer += Time.deltaTime;
            if (_shootTimer >= ShootRate) Shoot();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        //if (other.gameObject.TryGetComponent()) Component for enemy
        {
            _enemiesInRange.Add(other.gameObject); //add the component
        }
    }

    private void OnCollisionExit(Collision other)
    {
        //if (other.gameObject.TryGetComponent()) Component for enemy
        {
            _enemiesInRange.Remove(other.gameObject); //add the component
        }
    }
}