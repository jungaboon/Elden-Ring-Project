using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Malenia : Enemy
{
    [SerializeField] private BoxCollider swordCollider;
    private IEnumerator attackCoroutine;

    public override void Update()
    {
        base.Update();
        LookAtTarget(0.2f);
    }

    public void StartAttack(float duration)
    {
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = Attack(duration);
        StartCoroutine(attackCoroutine);
    }
    private IEnumerator Attack(float duration)
    {
        swordCollider.enabled = true;
        yield return new WaitForSeconds(duration);
        swordCollider.enabled = false;
    }
}
