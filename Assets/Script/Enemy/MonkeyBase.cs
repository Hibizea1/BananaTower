using System;
using UnityEngine;

public abstract class MonkeyBase : MonoBehaviour
{
    [SerializeField] protected int _damage;
    [SerializeField] protected int _health;
    [SerializeField] protected int _bananasOnDeath;
    [SerializeField] protected float _speed;

    #region PropertySettings

    public int Damage
    {
        get => _damage;
        protected set => _damage = value;
    }

    public int Health
    {
        get => _health;
        protected set => _health = value;
    }

    public int BananasOnDeath
    {
        get => _bananasOnDeath;
        protected set => _bananasOnDeath = value;
    }

    public float Speed
    {
        get => _speed;
        protected set => _speed = value;
    }

    #endregion

    void Awake()
    {
        EventMaster.GetInstance().CreateNewEvent("SetUpPath");
    }

    private void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        //TODO : movement Here
    }

    public virtual void TakeDamage(int damageTaken)
    {
        if (_health - damageTaken > 0)
            _health -= damageTaken;
        else
            Die();
    }

    protected void Die()
    {
        //TODO : monkey death
        EventMaster.GetInstance().InvokeEventInt("AddMoney", _bananasOnDeath);
        Destroy(gameObject);
    }

    protected virtual void DealDamage()
    {
        
    }

    public abstract void SpecialAbility();
}