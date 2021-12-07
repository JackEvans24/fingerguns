using UnityEngine;

public class HealthCollider : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private float damageMultiplier;

    public void TakeDamage(float damage)
    {
        this.health.TakeDamage(damage * this.damageMultiplier);
    }
}