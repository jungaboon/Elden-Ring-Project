// Created by Ronis Vision. All rights reserved
// 05.11.2019.

using System;
using RVModules.RVUtilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RVModules.RVLoadBalancer
{
    /// <summary>
    /// Exactly the same as LoadBalancerSingleton, but works in fixed update loop (more reliable/framerate independent timings)
    /// </summary>
    public class LoadBalancerFixedSingleton : MonoSingleton<LoadBalancerFixedSingleton>
    {
        #region Fields

        private LoadBalancerManager loadBalancerManager;
        private bool debugMode;

        #endregion

        #region Properties

        public override string Name => "LoadBalancerFixedManager";

        #endregion

        #region Public methods

        public void EnableDebug()
        {
            if (debugMode) return;
            loadBalancerManager.EnableDebug(gameObject, false);
            debugMode = true;
        }

        /// <summary>
        /// Register new Action that will be called every frame (no load balancing)
        /// </summary>
        public void Register(object _object, Action<float> _action) =>
            loadBalancerManager.Register(_object, _action, new LoadBalancerConfig(LoadBalancerType.EveryXFrames, 0));

        /// <summary>
        /// Register new Action
        /// Object is owner of action
        /// _action is method to call at _frequency (n time per second)
        /// </summary>
        public void Register(object _object, Action<float> _action, int _frequency, bool _calculateDeltaTime = false, bool _useUnscaledDt = false) =>
            loadBalancerManager.Register(_object, _action, _frequency, _calculateDeltaTime, _useUnscaledDt);

        public void Register(object _object, Action<float> _action, LoadBalancerConfig _lbc) => loadBalancerManager.Register(_object, _action, _lbc);

        /// <summary>
        /// Removes all actions registered by _object
        /// </summary>
        public void Unregister(object _object) => loadBalancerManager.Unregister(_object);

        /// <summary>
        /// Removes only passed _action added by _object
        /// </summary>
        public void Unregister(object _object, Action<float> _action)
        {
            if (!loadBalancerManager.Unregister(_object, _action)) Debug.LogError($"Failed to unregister {_object}", _object as Object);
        }
        
        /// <summary>
        /// Static overload that automatically handles null instance (on scene unload/game exit)
        /// </summary>
        public static void UnregisterStatic(object _object)
        {
            if (instance == null) return;
            instance.loadBalancerManager.Unregister(_object);
        }
        
        /// <summary>
        /// Static overload that automatically handles null instance (on scene unload/game exit)
        /// </summary>
        public static void UnregisterStatic(object _object, Action<float> _action)
        {
            if (instance == null) return;
            instance.loadBalancerManager.Unregister(_object, _action);
        }

        #endregion

        #region Not public methods

        protected override void SingletonInitialization() => loadBalancerManager = new LoadBalancerManager("LoadBalancerFixedSingleton");

        private void FixedUpdate() => loadBalancerManager.Tick(Time.fixedDeltaTime, Time.fixedUnscaledDeltaTime);

        #endregion
    }
}