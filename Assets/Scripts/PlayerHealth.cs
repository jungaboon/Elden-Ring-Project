using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerHealth : Health
{
    private Animator animator;
    private CinemachineImpulseSource impulse;

    public override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        impulse = GetComponent<CinemachineImpulseSource>();
    }

    public override void Damage(float amount)
    {
        base.Damage(amount);
        impulse.GenerateImpulse();
        float hurtType = Mathf.Round(Random.Range(0f, 3f));
        animator.SetFloat("hurtType", hurtType);
        animator.Play("Hurt");
    }
}
