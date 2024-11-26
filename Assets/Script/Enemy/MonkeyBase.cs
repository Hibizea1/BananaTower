using UnityEngine;

public abstract class MonkeyBase : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private int _health;
    [SerializeField] private int _bananasOnDeath;
    [SerializeField] private float _speed;

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

    private void Die()
    {
        //TODO : monkey death
        Destroy(this);
    }

    public abstract void SpecialAbility();
}