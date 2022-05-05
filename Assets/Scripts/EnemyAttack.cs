using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float attackDamage = 20f;
    public bool heavyAttack;
    private VFXManager vfxManager;
    private Collider coll;

    private void Start()
    {
        vfxManager = VFXManager.Instance;
        coll = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("On Trigger enter " + other.name);
        if(other.TryGetComponent(out Health health))
        {
            float dotDirection = Mathf.Round(Vector3.Dot(transform.forward, other.transform.forward));
            health.Damage(attackDamage, heavyAttack, dotDirection);
        }
    }
}
