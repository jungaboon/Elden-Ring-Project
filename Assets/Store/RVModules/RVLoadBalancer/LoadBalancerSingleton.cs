// Created by Ronis Vision. All rights reserved
// 05.11.2019.

using System;
using RVModules.RVUtilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RVModules.RVLoadBalancer
{
    /// <summary>
    /// Allows easy setting of frequency on per method basis.
    /// This allows for not only for calling on every object with different frequency, but it also means, all actions with same frequency will be
    /// load balanced(not updating all collection at once, but only some part of it per frame, to spread work nicely)
    /// This api is super fast, based on dictionaries, so many Register/Unregister calls are not problem.
    /// Usage:
    /// Call register on awake/start/onEnable as you prefer
    /// LoadBalancerManager.Instance.Register(this, OncePerSecUpdate, 1);
    /// Call unregister when object is destroyed or disabled - OnDestroy/OnDisable
    /// LoadBalancerManager.Instance.Unregister(this);
    /// You can also turn off single methods, and enable them again later
    /// LoadBalancerManager.Instance.Unregister(this, OncePerSecUpdate);
    ///
    /// Enable LB_PERF_DEBUG directive to enable performance debugging - it will log how much each load balancer takes time to execute in debug logs,
    /// can also filter to log only if load balancer took more than x ms - see static logLoadBalancersTakingLongerThanMs field 
    /// </summary>
    public class LoadBalancerSingleton : MonoSingleton<LoadBalancerSingleton>
    {
        #region Fields

        private LoadBalancerManager loadBalancerManager;
        private bool debugMode;

        /// <summary>
        /// Relevant only with LB_PERF_DEBUG preprocessor directive
        /// </summary>
        public static double logLoadBalancersTakingLongerThanMs = 0.00;

        #endregion

        #region Properties

        public override string Name => "LoadBalancerManager";

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

        /// <summary>
        /// Removes only passed _action added by _object
        /// </summary>
        public void Unregister(object _object, Action<float> _action)
        {
            if (!loadBalancerManager.Unregister(_object, _action)) Debug.LogError($"Failed to unregister {_object}", _object as Object);
        }

        #endregion

        #region Not public methods

        protected override void SingletonInitialization() => loadBalancerManager = new LoadBalancerManager("LoadBalancerSingleton");

        private void Update() => loadBalancerManager.Tick(Time.deltaTime, Time.unscaledDeltaTime);

        #endregion
    }
}