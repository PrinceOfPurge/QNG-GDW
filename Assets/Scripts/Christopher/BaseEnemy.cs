using UnityEngine;
using System.Collections;

public abstract class BaseEnemy : MonoBehaviour
{

    public abstract void attack();
    [SerializeField] protected float health = 25f;

    [SerializeField]
    private float maxHealth = 100.0f;
    [SerializeField]
    private float timeToSelfDestruct = 3.0f;

    void Start()
    {
        health = maxHealth;
    }
    void OnEnable()
    {
        attack();

    }
    private void OnDisable()
    {
        ResetDrone();
    }
    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(timeToSelfDestruct);
        TakeDamage(maxHealth);
    }

    // return an available instance in the pool
    private void ReturnToPool()
    {
        EnemyWaveManager.Instance.ReturnEnemy(this.gameObject);
    }
    private void ResetDrone()
    {
        health = maxHealth;
    }
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0.0f)
            ReturnToPool();
    }
    



}
