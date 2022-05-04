using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float attackDamage = 20f;
    private VFXManager vfxManager;
    private Collider coll;

    private void Start()
    {
        vfxManager = VFXManager.Instance;
        coll = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Health health))
        {
            Debug.Log("Hit " + other.name);
            health.Damage(attackDamage);
            //Vector3 midpoint = Vector3.Lerp(other.bounds.center, coll.bounds.center, 0.5f);
            //vfxManager.SpawnParticle(ParticleType.Hitspark, midpoint);
        }
    }
}
