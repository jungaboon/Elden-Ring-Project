// Created by Ronis Vision. All rights reserved
// 23.08.2020.

using UnityEngine;

namespace RVModules.RVLoadBalancer
{
    /// <summary>
    /// Just like MonoBehaviour, but better ;)
    /// </summary>
    public abstract class LoadBalancedBehaviour : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        [Tooltip("Don't change at runtime, use API or disable and enable this component after changing to update")]
        private bool useFixedUpdateLoop;

//        [SerializeField]
//        [Tooltip("Behaviour will be registered automatically in OnEnable. If you want better control over initialization, set this to false and" +
//                 "call Initialize manually when you want this to start working")]
//        private bool initializeOnEnable = true;
//
//        private bool initialized = false;

        private bool registeredFixedUpdate;
        private bool registered;

//        public void Initialize()
//        {
//            if (initialized) return;
//            if (enabled) RegisterUpdateLoop();
//            initialized = true;
//        }

        #endregion

        #region Properties

        /// <summary>
        /// LBC for LoadBalancedUpdate method. Create serialized field with LoadBalancerConfig if you want to expose it in inspector
        /// Default implementation returns default-ctor lbc (every frame, no delta time calc)
        /// </summary>
        protected virtual LoadBalancerConfig LoadBalancerConfig => new LoadBalancerConfig();

        /// <summary>
        /// Can be set at runtime at anytime
        /// </summary>
        public bool UseFixedUpdateLoop
        {
            get => useFixedUpdateLoop;
            protected set
            {
                if (useFixedUpdateLoop == value) return;
                useFixedUpdateLoop = value;

                if (!registered) return;

                UnregisterUpdateLoop();
                RegisterUpdateLoop();
            }
        }

        #endregion

        #region Not public methods

        /// <summary>
        /// Register LoadBalancedUpdate
        /// </summary>
        protected virtual void OnEnable()
        {
//            if (!initialized)
//            {
//                if (initializeOnEnable) Initialize();
//                else
//                    return;
//            }

            RegisterUpdateLoop();
        }

        /// <summary>
        /// Unregister LoadBalancedUpdate
        /// </summary>
        protected virtual void OnDisable() => UnregisterUpdateLoop();

        protected void RegisterUpdateLoop()
        {
            if (useFixedUpdateLoop) LoadBalancerFixedSingleton.Instance.Register(this, LoadBalancedUpdate, LoadBalancerConfig);
            else
                LoadBalancerSingleton.Instance.Register(this, LoadBalancedUpdate, LoadBalancerConfig);

            registeredFixedUpdate = useFixedUpdateLoop;
            registered = true;
        }

        protected void UnregisterUpdateLoop()
        {
            if (registeredFixedUpdate) LoadBalancerFixedSingleton.UnregisterStatic(this, LoadBalancedUpdate);
            else LoadBalancerSingleton.UnregisterStatic(this, LoadBalancedUpdate);
            registered = false;
        }

        /// <summary>
        /// Unregisters all load balanced methods registered by this object
        /// </summary>
        protected void UnregisterAllLbMethods()
        {
            LoadBalancerSingleton.UnregisterStatic(this);
            LoadBalancerFixedSingleton.UnregisterStatic(this);
            registered = false;
        }

        protected abstract void LoadBalancedUpdate(float _deltaTime);

        #endregion
    }
}