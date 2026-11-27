using UnityEngine;

public sealed class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maximumHealth = 100f;
    [SerializeField] private float damageCooldown = 0.65f;

    private float currentHealth;
    private float nextDamageTime;

    public float CurrentHealth => currentHealth;
    public float MaximumHealth => maximumHealth;

    private void Awake()
    {
        currentHealth = maximumHealth;
    }

    public void TakeDamage(float amount)
    {
        if (GameManager.IsPaused || GameManager.IsGameOver || Time.time < nextDamageTime || amount <= 0f)
            return;

        nextDamageTime = Time.time + damageCooldown;
        currentHealth = Mathf.Max(0f, currentHealth - amount);

        if (currentHealth <= 0f && GameManager.Instance != null)
            GameManager.Instance.SetGameOver();
    }

    public void RestoreFullHealth()
    {
        currentHealth = maximumHealth;
        nextDamageTime = 0f;
    }
}
