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

    public virtual void Damage(float amount, bool heavy = false, float dotDirection = 0f)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f) Die();
    }

    public virtual void Die()
    {

    }
}
