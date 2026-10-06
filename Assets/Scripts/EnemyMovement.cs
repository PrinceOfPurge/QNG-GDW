using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stoppingDistance = 1.5f;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;

    private Transform target;
    private GameObject player;
    private float nextAttackTime;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Truck").transform;
        player = GameObject.FindGameObjectWithTag("Player");

        if (target == null)
        {
            Debug.LogError("Truck tag not found");
        }
    }

    private void Update()
    {
        if (target == null)
            return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackRange)
        {
            Attack();
        }
        else
        {
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        float distance = Vector3.Distance(transform.position,target.position);

        if (distance <= stoppingDistance)
            return;

        Vector3 direction = (target.position - transform.position).normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;

        FaceTarget();
    }

    private void Attack()
    {
        FaceTarget();

        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + attackCooldown;

        Debug.Log(gameObject.name + ". Damage delt: " + attackDamage);
        player.GetComponent<IDamageable>().TakeDamage(attackDamage);
    }

    private void FaceTarget()
    {
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
    }
}
