using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : BaseEnemy, IDamageable
{
    [SerializeField] private float health = 25f;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public override void attack()
    {
        Debug.Log("zombie attacks");
    }
}
