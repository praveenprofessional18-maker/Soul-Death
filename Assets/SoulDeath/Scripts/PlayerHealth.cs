using System;
using UnityEngine;

public sealed class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField, Min(1f)] private float maximumHealth = 100f;
    [SerializeField, Min(0f)] private float damageCooldown = 0.65f;

    private float currentHealth;
    private float nextDamageTime;
    private bool isDead;

    public float CurrentHealth => currentHealth;
    public float MaximumHealth => maximumHealth;
    public bool IsDead => isDead;

    public event Action<float, float> HealthChanged;
    public event Action PlayerDied;

    private void Awake()
    {
        maximumHealth = Mathf.Max(1f, maximumHealth);
        damageCooldown = Mathf.Max(0f, damageCooldown);

        currentHealth = maximumHealth;
        HealthChanged?.Invoke(currentHealth, maximumHealth);
    }

    public void TakeDamage(float amount)
    {
        if (isDead || GameManager.IsPaused || GameManager.IsGameOver ||
            Time.time < nextDamageTime || amount <= 0f ||
            float.IsNaN(amount) || float.IsInfinity(amount))
        {
            return;
        }

        nextDamageTime = Time.time + damageCooldown;
        SetHealth(currentHealth - amount);

        if (currentHealth <= 0f)
        {
            isDead = true;
            PlayerDied?.Invoke();

            if (GameManager.Instance != null)
                GameManager.Instance.SetGameOver();
        }
    }

    public void Heal(float amount)
    {
        if (isDead || amount <= 0f ||
            float.IsNaN(amount) || float.IsInfinity(amount))
        {
            return;
        }

        SetHealth(currentHealth + amount);
    }

    public void RestoreFullHealth()
    {
        if (isDead)
            return;

        SetHealth(maximumHealth);
        nextDamageTime = 0f;
    }

    private void SetHealth(float value)
    {
        float updatedHealth = Mathf.Clamp(value, 0f, maximumHealth);

        if (Mathf.Approximately(currentHealth, updatedHealth))
            return;

        currentHealth = updatedHealth;
        HealthChanged?.Invoke(currentHealth, maximumHealth);
    }
}
