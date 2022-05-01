using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Collider coll;
    private VFXManager vfxManager;
    private void Start()
    {
        vfxManager = VFXManager.Instance;
        coll = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 midpoint = Vector3.Lerp(other.bounds.center, coll.bounds.center, 0.5f);
        vfxManager.SpawnParticle(ParticleType.Hitspark, midpoint);
    }
}
