// Created by Ronis Vision. All rights reserved
// 10.09.2020.

using System;
using UnityEngine;

namespace RVModules.RVUtilities
{
    /// <summary>
    /// MonoBehaviour with events for common unity event methods like OnEnable, OnDestroy etc. Allows for easier/better handling of those with inheritance
    /// and also for other objects to subscribe to them.
    /// If used as base class dont use other unity event methods than Awake - they are used by this class, subscribe to events instead.
    /// </summary>
    public class MonoBehaviourEvents : MonoBehaviour
    {
        public event Action onStart, onEnable, onDisable, onBecomeInvisible, onBecomeVisible, onDestroy;

        private void Start() => onStart?.Invoke();

        private void OnEnable() => onEnable?.Invoke();

        private void OnDisable() => onDisable?.Invoke();

        private void OnBecameVisible() => onBecomeVisible?.Invoke();

        private void OnBecameInvisible() => onBecomeInvisible?.Invoke();

        private void OnDestroy() => onDestroy?.Invoke();
    }
}