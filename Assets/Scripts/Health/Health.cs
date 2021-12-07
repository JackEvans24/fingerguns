using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private int maxHealth;

    private int currentHealth;

    public bool Dead { get => this.currentHealth <= 0; }

    private void Awake()
    {
        this.ResetHealth();
    }

    public void ResetHealth()
    {
        this.currentHealth = this.maxHealth;      
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0)
            return;

        currentHealth = Mathf.Max(0, (int)Mathf.Ceil(currentHealth - damage));

        if (currentHealth <= 0)
            this.Die();
    }

    public void Die()
    {
        if (this.player == null)
            return;

        this.player.Die();
    }
}
