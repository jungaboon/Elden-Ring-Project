using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    public bool lockedOn;

    [SerializeField] private Image lockOnReticle;

    private void Awake()
    {
        Instance = this;
        lockOnReticle.enabled = false;
    }
    private void OnEnable()
    {
        GameEventManager.Instance.onLockOn += OnLockOn;
    }
    private void OnDisable()
    {
        GameEventManager.Instance.onLockOn -= OnLockOn;
    }
    public void OnLockOn(bool active)
    {
        lockedOn = active;
        lockOnReticle.enabled = active;
    }
    public void MoveLockOnReticle(Vector3 position)
    {
        if(lockedOn)
        {
            lockOnReticle.transform.position = position;
        }
    }
}
