using Photon.Pun;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private ParticleSystem hurtParticles;
    [SerializeField] private HitIndicator indicator;
    [SerializeField] private SoundFromArray hurtSound;

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
        this.CurrentHealth = this.MaxHealth;

        if (!player.View.IsMine)
            return;

        this.healthBar.ResetHealth(this); 
    }

    public void TakeDamage(float damage, Transform damagedBy)
    {
        player.View.RPC(nameof(this.RPC_TakeDamage), RpcTarget.All, damage, damagedBy.position);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, Vector3 damagedFrom)
    {
        if (this.CurrentHealth <= 0)
            return;

        this.hurtParticles.Play();

        this.hurtSound.Play();

        this.CurrentHealth = Mathf.Max(0, (int)Mathf.Ceil(this.CurrentHealth - damage));

        if (!player.View.IsMine)
            return;

        this.indicator.ShowHitFrom(damagedFrom);

        this.healthBar.UpdateHealth(this);

        if (this.CurrentHealth <= 0)
            this.Die();
    }

    public void Die()
    {
        if (this.player == null)
            return;

        var properties = this.player.View.Owner.CustomProperties;

        if (properties.TryGetValue("Deaths", out object storedDeaths))
            properties["Deaths"] = (int)storedDeaths + 1;
        else
            properties.Add("Deaths", 1);

        this.player.View.Owner.SetCustomProperties(properties);

        this.player.Die();
    }
}
