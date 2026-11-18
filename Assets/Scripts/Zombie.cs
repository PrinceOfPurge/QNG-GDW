using UnityEngine;

public class Zombie : MonoBehaviour, IDamageable, IResettable
{
    [SerializeField] private float health = 25f;
    private float _maxHealth;
    private PooledEnemy _pooled;
    private void Awake()
    {
        _maxHealth = health;
    }

    public void TakeDamage(float damage)
    {
        if (health <= 0f) return;
        health -= damage;
        if (health <= 0f)
        {
            Die();
        }
    }
    public void ResetForReuse()
    {
        health = _maxHealth;
    }

    private void Die()
    {
        if (_pooled == null) _pooled = GetComponent<PooledEnemy>();
        if (_pooled != null) _pooled.ReturnToPool();
        else Destroy(gameObject);
    }
}
