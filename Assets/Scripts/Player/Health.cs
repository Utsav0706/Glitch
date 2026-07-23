using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;

    public event Action<float> Damaged;
    public event Action Died;

    float current;
    bool dead;

    public float Current => current;
    public float Max => maxHealth;
    public float Normalized => maxHealth > 0f ? current / maxHealth : 0f;
    public bool IsDead => dead;

    void Awake()
    {
        current = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (dead || amount <= 0f) return;
        current = Mathf.Max(0f, current - amount);
        Damaged?.Invoke(amount);
        if (current <= 0f) Die();
    }

    public void Heal(float amount)
    {
        if (dead || amount <= 0f) return;
        current = Mathf.Min(maxHealth, current + amount);
    }

    public void Revive()
    {
        dead = false;
        current = maxHealth;
    }

    void Die()
    {
        dead = true;
        Died?.Invoke();
    }
}
