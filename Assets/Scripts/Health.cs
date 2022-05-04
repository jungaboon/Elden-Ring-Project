using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

    public virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void Damage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f) Die();
    }

    public virtual void Die()
    {

    }
}
