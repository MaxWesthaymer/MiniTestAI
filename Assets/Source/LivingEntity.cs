using System;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float Health { get; protected set; }
    public float Speed { get; protected set; }
    public float AttackPower { get; protected set; }
    public float Defence { get; protected set; }
    private float _currentHealth;
    protected bool dead;

    public event Action OnDeath;

    protected virtual void Start()
    {
        _currentHealth = Health;
    }

    public void TakeHit(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    public void SetupEntity(EntityConfig config)
    {
        Health = config.health;
        Speed = config.speed;
        AttackPower = config.attackPower;
        Defence = config.defence;
    }
    
    private void Die()
    {
        dead = true;
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}

