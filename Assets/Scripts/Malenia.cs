using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class Malenia : Enemy
{
    [SerializeField] private BoxCollider swordCollider;
    [Header("Torso Aim")]
    [SerializeField] private MultiAimConstraint torsoAimConstraint;
    [SerializeField] private Transform torsoAimObject;
    private IEnumerator attackCoroutine;

    public override void Update()
    {
        base.Update();
        LookAtTarget(0.2f);
        AimAtTarget();
    }

    private void AimAtTarget()
    {
        if(attacking)
        {
            torsoAimConstraint.weight += 3f * Time.deltaTime;
            torsoAimObject.position = Vector3.MoveTowards(torsoAimObject.position, target.transform.position, 0.35f);
        }
        else
        {
            torsoAimConstraint.weight -= 3f * Time.deltaTime;
        }
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
