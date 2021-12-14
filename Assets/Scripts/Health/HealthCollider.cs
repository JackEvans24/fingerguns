using UnityEngine;

public class HealthCollider : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private float damageMultiplier;

    public bool Dead { get => this.health.Dead; }

    public bool TryKill(float damage, Transform damagedBy)
    {
        var totalDamage = damage * this.damageMultiplier;
        var killed = health.CurrentHealth - totalDamage <= 0;

        this.health.TakeDamage(totalDamage, damagedBy);

        return killed;
    }
}
