using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerHealth : Health
{
    private Animator animator;
    private CinemachineImpulseSource impulse;
    private VFXManager vfxManager;

    public bool canTakeDamage;

    public override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        impulse = GetComponent<CinemachineImpulseSource>();
        vfxManager = VFXManager.Instance;

        canTakeDamage = true;
    }

    public override void Damage(float amount, bool heavy = false, float dotDirection = 0f)
    {
        if (!canTakeDamage) return;

        base.Damage(amount);
        vfxManager.SpawnParticle(ParticleType.Hitspark, transform.position + Vector3.up);
        impulse.GenerateImpulse();

        if(heavy)
        {
            animator.SetFloat("hitDirection", dotDirection);
            animator.Play("Knockdown", 0, 0f);
        }
        else
        {
            float hurtType = Mathf.Round(Random.Range(0f, 3f));
            animator.SetFloat("hurtType", hurtType);
            animator.Play("Hurt", 0, 0f);
        }
    }
}
