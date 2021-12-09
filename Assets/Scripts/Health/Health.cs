using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private HealthBar healthBar;

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

    public void TakeDamage(float damage)
    {
        player.View.RPC(nameof(this.RPC_TakeDamage), RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        if (!player.View.IsMine)
            return;
        if (this.CurrentHealth <= 0)
            return;

        this.CurrentHealth = Mathf.Max(0, (int)Mathf.Ceil(this.CurrentHealth - damage));
        this.healthBar.UpdateHealth(this);

        Debug.Log($"I have {this.CurrentHealth} health");

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
