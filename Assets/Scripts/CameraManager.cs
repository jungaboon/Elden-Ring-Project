using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook freeLookCamera;
    [SerializeField] private CinemachineVirtualCamera lockOnCamera;

    private void OnEnable()
    {
        GameEventManager.Instance.onLockOn += ChangeCamera;
    }
    private void OnDisable()
    {
        GameEventManager.Instance.onLockOn -= ChangeCamera;
    }
    private void ChangeCamera(bool active)
    {
        lockOnCamera.enabled = active;
        freeLookCamera.enabled = !active;
    }
}
