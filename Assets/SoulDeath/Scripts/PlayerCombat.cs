using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float damage = 28f;
    [SerializeField] private float reach = 2.1f;
    [SerializeField] private float attackRadius = 0.85f;
    [SerializeField] private float attackCooldown = 0.48f;

    private float nextAttackTime;
    private float lastAttackTime = -10f;

    public float TimeSinceLastAttack => Time.time - lastAttackTime;

    private void Update()
    {
        if (GameManager.IsPaused || GameManager.IsGameOver || Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextAttackTime)
            Attack();
    }

    private void Attack()
    {
        nextAttackTime = Time.time + attackCooldown;
        lastAttackTime = Time.time;

        Vector3 center = transform.position + Vector3.up * 0.95f + transform.forward * reach;
        Collider[] hits = Physics.OverlapSphere(center, attackRadius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        HashSet<EnemyAI> damaged = new HashSet<EnemyAI>();

        foreach (Collider hit in hits)
        {
            EnemyAI enemy = hit.GetComponentInParent<EnemyAI>();
            if (enemy != null && damaged.Add(enemy))
                enemy.TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.9f, 0.12f, 0.16f, 0.55f);
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.95f + transform.forward * reach, attackRadius);
    }
}
