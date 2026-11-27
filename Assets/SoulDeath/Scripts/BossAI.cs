using UnityEngine;

public sealed class BossAI : EnemyAI
{
    [SerializeField] private bool defensive;
    [SerializeField, Range(0f, 1f)] private float guardedDamageMultiplier = 0.45f;

    public bool IsDefensive => defensive;

    public void Configure(bool useDefensiveStyle)
    {
        defensive = useDefensiveStyle;
    }

    public override void TakeDamage(float amount)
    {
        if (defensive && Time.time < nextAttackTime)
            amount *= guardedDamageMultiplier;

        base.TakeDamage(amount);
    }
}
