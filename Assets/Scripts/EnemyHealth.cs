using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyHealth : Health
{
    private Animator animator;
    private IEnumerator hitCoroutine;

    private WaitForSeconds hitEffectDelay = new WaitForSeconds(0.15f);

    public override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    public override void Damage(float amount)
    {
        base.Damage(amount);
        if (hitCoroutine != null) StopCoroutine(hitCoroutine);
        hitCoroutine = HitEffect();
        StartCoroutine(hitCoroutine);
    }

    private IEnumerator HitEffect()
    {
        transform.DORewind();
        transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 0.15f);
        animator.speed = 0f;
        yield return hitEffectDelay;
        animator.speed = 1f;
    }
}
