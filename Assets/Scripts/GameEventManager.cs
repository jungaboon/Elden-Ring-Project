using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public event Action<bool> onLockOn;
    public void LockOn(bool active)
    {
        if (onLockOn != null) onLockOn(active);
    }
}
