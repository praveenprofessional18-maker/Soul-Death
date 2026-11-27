using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyAI : MonoBehaviour, IDamageable
{
    public string displayName = "Enemy";

    [SerializeField] protected float maximumHealth = 85f;
    [SerializeField] protected float movementSpeed = 2.4f;
    [SerializeField] protected float aggroRange = 18f;
    [SerializeField] protected float attackRange = 1.65f;
    [SerializeField] protected float attackDamage = 12f;
    [SerializeField] protected float attackCooldown = 1.45f;

    protected CharacterController movementController;
    protected PlayerHealth target;
    protected float currentHealth;
    protected float nextAttackTime;
    protected float verticalSpeed;
    protected bool defeated;

    public float CurrentHealth => currentHealth;
    public float MaximumHealth => maximumHealth;
    public bool IsDefeated => defeated;

    public void ConfigureStats(float health, float speed, float range, float damage, float cooldown)
    {
        maximumHealth = health;
        movementSpeed = speed;
        aggroRange = range;
        attackDamage = damage;
        attackCooldown = cooldown;
        currentHealth = maximumHealth;
    }

    protected virtual void Awake()
    {
        movementController = GetComponent<CharacterController>();
        currentHealth = maximumHealth;
    }

    protected virtual void Update()
    {
        if (GameManager.IsPaused || GameManager.IsGameOver || defeated)
            return;

        if (target == null)
            target = FindAnyObjectByType<PlayerHealth>();
        if (target == null)
            return;

        Vector3 offset = target.transform.position - transform.position;
        offset.y = 0f;
        float distance = offset.magnitude;

        if (distance > aggroRange)
            return;

        if (distance > attackRange && distance > 0.05f)
        {
            Vector3 direction = offset / distance;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 10f * Time.deltaTime);
            movementController.Move(direction * movementSpeed * Time.deltaTime + Vector3.up * verticalSpeed * Time.deltaTime);
        }
        else if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            target.TakeDamage(attackDamage);
        }

        if (movementController.isGrounded)
            verticalSpeed = -2f;
        else
            verticalSpeed += Physics.gravity.y * Time.deltaTime;
    }

    public virtual void TakeDamage(float amount)
    {
        if (defeated || amount <= 0f)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        if (currentHealth <= 0f)
            Defeat();
    }

    protected virtual void Defeat()
    {
        defeated = true;
        movementController.enabled = false;

        BossAI boss = this as BossAI;
        if (boss != null && GameManager.Instance != null)
            GameManager.Instance.RegisterBossDefeat();

        Destroy(gameObject, 0.25f);
    }
}
