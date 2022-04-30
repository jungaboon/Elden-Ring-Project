// Created by Ronis Vision. All rights reserved
// 04.09.2020.

using System;

namespace RVModules.RVLoadBalancer
{
    /// <summary>
    /// Convenience class to avoid boilerplaet of writing LoadBalancerSingleton.Instance
    /// </summary>
    public static class LB
    {
        /// <summary>
        /// Register new Action that will be called every frame (no load balancing)
        /// </summary>
        public static void Register(object _object, Action<float> _action) =>
            LoadBalancerSingleton.Instance.Register(_object, _action, new LoadBalancerConfig(LoadBalancerType.EveryXFrames, 0));

        /// <summary>
        /// Register new Action
        /// Object is owner of action
        /// _action is method to call at _frequency (n time per second)
        /// </summary>
        public static void Register(object _object, Action<float> _action, int _frequency, bool _calculateDeltaTime = false, bool _useUnscaledDt = false) =>
            LoadBalancerSingleton.Instance.Register(_object, _action, _frequency, _calculateDeltaTime, _useUnscaledDt);

        public static void Register(object _object, Action<float> _action, LoadBalancerConfig _lbc) =>
            LoadBalancerSingleton.Instance.Register(_object, _action, _lbc);

        /// <summary>
        /// Removes all actions registered by _object
        /// </summary>
        public static void Unregister(object _object) => LoadBalancerSingleton.UnregisterStatic(_object);

        /// <summary>
        /// Removes only passed _action added by _object
        /// </summary>
        public static void Unregister(object _object, Action<float> _action) => LoadBalancerSingleton.UnregisterStatic(_object, _action);
    }
    
    /// <summary>
    /// Convenience class to avoid boilerplaet of writing LoadBalancerFixedSingleton.Instance
    /// </summary>
    public static class LBF
    {
        /// <summary>
        /// Register new Action that will be called every frame (no load balancing)
        /// </summary>
        public static void Register(object _object, Action<float> _action) =>
            LoadBalancerFixedSingleton.Instance.Register(_object, _action, new LoadBalancerConfig(LoadBalancerType.EveryXFrames, 0));

        /// <summary>
        /// Register new Action
        /// Object is owner of action
        /// _action is method to call at _frequency (n time per second)
        /// </summary>
        public static void Register(object _object, Action<float> _action, int _frequency, bool _calculateDeltaTime = false, bool _useUnscaledDt = false) =>
            LoadBalancerFixedSingleton.Instance.Register(_object, _action, _frequency, _calculateDeltaTime, _useUnscaledDt);

        public static void Register(object _object, Action<float> _action, LoadBalancerConfig _lbc) =>
            LoadBalancerFixedSingleton.Instance.Register(_object, _action, _lbc);

        /// <summary>
        /// Removes all actions registered by _object
        /// </summary>
        public static void Unregister(object _object) => LoadBalancerFixedSingleton.UnregisterStatic(_object);

        /// <summary>
        /// Removes only passed _action added by _object
        /// </summary>
        public static void Unregister(object _object, Action<float> _action) => LoadBalancerFixedSingleton.UnregisterStatic(_object, _action);
    }
}