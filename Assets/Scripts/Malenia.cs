using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class Malenia : Enemy
{
    [Header("Attack Parameters")]
    [SerializeField] private Transform weapon;
    [SerializeField] private Vector3 weaponHitboxSize;
    [SerializeField] private Vector3 weaponHitboxPosition;
    [SerializeField] private LayerMask attackLayer;

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
        float endTime = Time.time + duration;
        bool hitTarget = false;
        while (Time.time < endTime)
        {
            Vector3 pos = weapon.position + (weapon.right * weaponHitboxPosition.x) + (weapon.up * weaponHitboxPosition.y) + (weapon.forward * weaponHitboxPosition.z);
            Collider[] coll = Physics.OverlapBox(pos, weaponHitboxSize, weapon.transform.rotation, attackLayer);
            for (int i = 0; i < coll.Length; i++)
            {
                if (coll[i].TryGetComponent(out Health health))
                {
                    health.Damage(attackDamage, heavyAttack, Vector3.Dot(transform.forward, coll[i].transform.forward));
                    Debug.Log("Hit " + coll[i].name);
                    hitTarget = true;
                    break;
                }
            }
            if (hitTarget) break;
            yield return null;
        }
        Debug.Log("Finish coroutine");
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = weapon.localToWorldMatrix;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero + weaponHitboxPosition, weaponHitboxSize * 2f);
    }
}
