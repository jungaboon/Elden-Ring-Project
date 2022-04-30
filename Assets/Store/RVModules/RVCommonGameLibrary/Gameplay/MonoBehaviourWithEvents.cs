// Created by Ronis Vision. All rights reserved
// 07.03.2021.

using UnityEngine;
using UnityEngine.Events;

namespace RVModules.RVCommonGameLibrary.Gameplay
{
    /// <summary>
    /// Allows for external components to subscribe to this class' events like OnEnable etc  
    /// </summary>
    public class MonoBehaviourWithEvents : MonoBehaviour
    {
        #region Fields

        public UnityEvent onStart, onEnable, onDisable, onBecomeInvisible, onBecomeVisible, onDestroy;
        public ColliderUnityEvent onTriggerEnter, onTriggerStay, onTriggerExit;
        public CollisionUnityEvent onCollisionEnter, onCollisionStay, onCollisionExit;

        #endregion

        #region Not public methods

        private void Start() => onStart?.Invoke();

        private void OnEnable() => onEnable?.Invoke();

        private void OnDisable() => onDisable?.Invoke();

        private void OnBecameVisible() => onBecomeVisible?.Invoke();

        private void OnBecameInvisible() => onBecomeInvisible?.Invoke();

        private void OnDestroy() => onDestroy?.Invoke();

        private void OnTriggerEnter(Collider other) => onTriggerEnter?.Invoke(other);
        private void OnTriggerStay(Collider other) => onTriggerStay?.Invoke(other);
        private void OnTriggerExit(Collider other) => onTriggerExit?.Invoke(other);

        private void OnCollisionEnter(Collision other) => onCollisionEnter?.Invoke(other);
        private void OnCollisionStay(Collision other) => onCollisionStay?.Invoke(other);
        private void OnCollisionExit(Collision other) => onCollisionExit?.Invoke(other);

        #endregion
    }
}