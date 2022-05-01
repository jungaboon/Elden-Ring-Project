using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ParticleType
{
    Hitspark
}
public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    [Header("Effects")]
    [SerializeField] private ParticleSystem[] particleEffects;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnParticle(ParticleType type, Vector3 position = default(Vector3))
    {
        particleEffects[(int)type].transform.position = position;
        particleEffects[(int)type].Stop();
        particleEffects[(int)type].Play();
    }
}
