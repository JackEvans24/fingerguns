using Photon.Pun;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private ParticleSystem hurtParticles;
    [SerializeField] private HitIndicator indicator;

    [Header("Health Values")]
    public int MaxHealth;
    public int CurrentHealth;

    public bool Dead { get => this.CurrentHealth <= 0; }

    private void Awake()
    {
        this.ResetHealth();
    }

    public void ResetHealth()
    {
        if (!player.View.IsMine)
            return;

        this.CurrentHealth = this.MaxHealth;
        this.healthBar.ResetHealth(this); 
    }

    public void TakeDamage(float damage, Transform damagedBy)
    {
        player.View.RPC(nameof(this.RPC_TakeDamage), RpcTarget.All, damage, damagedBy.position);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, Vector3 damagedFrom)
    {
        this.hurtParticles.Play();

        if (!player.View.IsMine)
            return;
        if (this.CurrentHealth <= 0)
            return;

        this.indicator.ShowHitFrom(damagedFrom);

        this.CurrentHealth = Mathf.Max(0, (int)Mathf.Ceil(this.CurrentHealth - damage));
        this.healthBar.UpdateHealth(this);

        if (this.CurrentHealth <= 0)
            this.Die();
    }

    public void Die()
    {
        if (this.player == null)
            return;

        this.player.Die();
    }
}
