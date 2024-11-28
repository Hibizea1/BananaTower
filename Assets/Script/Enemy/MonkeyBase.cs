#region

using UnityEngine;

#endregion

public abstract class MonkeyBase : MonoBehaviour
{
    [SerializeField] int _damage;
    [SerializeField] int _health;
    [SerializeField] int _bananasOnDeath;
    [SerializeField] float _speed;


    void Update()
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

    void Die()
    {
        //TODO : monkey death
        Destroy(this);
    }

    public abstract void SpecialAbility();

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
}