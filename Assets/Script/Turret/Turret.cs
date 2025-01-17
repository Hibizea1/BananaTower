using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(CircleCollider2D))]
public abstract class Turret : MonoBehaviour
{
    [SerializeField] protected int _damage;
    [SerializeField] protected float _range;
    [SerializeField] protected Slider ReloadSlider;
    [SerializeField] protected int UpgradeCost;
    [SerializeField] protected int IncresedDamagePerUpgrade;
    [SerializeField] protected int IncresedRangePerUpgrade;
    [SerializeField] protected float DecreseReloadTimePerUpgrade;
    [SerializeField] protected int LevelCount;
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected float _timeToTimeToShoot;

    [SerializeField] protected TextMeshProUGUI CostText;

    [SerializeField] protected int _magazineSize;
    [SerializeField] protected float _reloadTime;

    #region PropertySettings

    public int Damage
    {
        get => _damage;
        set => _damage = value;
    }

    public float Range
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


    protected int _currentMagazine;

    protected float _reloadTimer;
    protected float _shootTimer;

    protected CircleCollider2D _detectionCollider;

    [SerializeField] protected List<MonkeyBase> _enemiesInRange;

    public List<MonkeyBase> EnemiesInRange { get; protected set; }

    protected virtual void Start()
    {
        _enemiesInRange = new List<MonkeyBase>();
        _detectionCollider = GetComponent<CircleCollider2D>();

        _currentMagazine = _magazineSize;
        _reloadTimer = 0;
        _shootTimer = 0;

        ReloadSlider.maxValue = _reloadTime;

        _detectionCollider.radius = _range;
    }

    void Update()
    {
        CheckOnReload();

        CheckOnShoot();
    }

    public void LoadData(int damage, float range, float shootRate, int magazineSize, float reloadTime, string loadName)
    {
        Damage = damage;
        Range = range;
        TimeToShoot = shootRate;
        MagazineSize = magazineSize;
        ReloadTime = reloadTime;
        name = loadName;
        enabled = true;
    }

    protected abstract void Shoot();
    public abstract void Upgrade();

    protected virtual void Reload()
    {
        _currentMagazine = MagazineSize;
        ReloadSlider.gameObject.SetActive(false);
    }

    protected virtual void CheckOnReload()
    {
        if (_currentMagazine <= 0)
        {
            ReloadSlider.gameObject.SetActive(true);
            _reloadTimer += Time.deltaTime;
            ReloadSlider.value = _reloadTimer;

            if (_reloadTimer >= ReloadTime)
            {
                Reload();
                _reloadTimer = 0.0f;
            }

        }
    }

    protected abstract void CheckOnShoot();


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

    public void SetCostText()
    {
        CostText.enabled = true;
        CostText.text =   "Upgrade cost : " + UpgradeCost.ToString();
    }
    public void ExitCostText()
    {
        CostText.enabled = false;
    }
}