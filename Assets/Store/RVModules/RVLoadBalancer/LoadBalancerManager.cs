// Created by Ronis Vision. All rights reserved
// 16.10.2020.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RVModules.RVLoadBalancer
{
    /// <summary>
    /// API NOT thread-safe!
    /// </summary>
    public class LoadBalancerManager
    {
        #region Fields

        public string name;

        private Queue<Dictionary<Action<float>, LoadBalancerConfig>> objectToActionsDictsPool = new Queue<Dictionary<Action<float>, LoadBalancerConfig>>();

        private Dictionary<object, Dictionary<Action<float>, LoadBalancerConfig>> objectToActionsMap =
            new Dictionary<object, Dictionary<Action<float>, LoadBalancerConfig>>();

        internal IDictionary<LoadBalancerConfig, LoadBalancer> loadBalancersDict =
            new ConcurrentDictionary<LoadBalancerConfig, LoadBalancer>(LoadBalancerConfig.LoadBalancerConfigComparer);

        private List<LoadBalancer> loadBalancersList = new List<LoadBalancer>(10);

        private bool debugMode;

#if LB_PERF_DEBUG
        private Stopwatch sw = Stopwatch.StartNew();
#else
        private Stopwatch sw;
#endif

        #endregion

        #region Properties

        public int LoadBalancersCount => loadBalancersDict.Count;

        /// <summary>
        /// For debugging only, dont modify returned dictionary!
        /// </summary>
        public IDictionary<LoadBalancerConfig, LoadBalancer> LoadBalancersDict => loadBalancersDict;

        #endregion

        public LoadBalancerManager(string _name) => name = _name;

        #region Public methods

        public bool TryGetLoadBalancer(LoadBalancerConfig _lbc, out LoadBalancer _loadBalancer) => loadBalancersDict.TryGetValue(_lbc, out _loadBalancer);

        /// <summary>
        /// Attaches debugger component to provided gameObject
        /// </summary>
        /// <param name="_gameObjectToAttachDebugger"></param>
        /// <param name="_destroyWhenEmpty"></param>
        public void EnableDebug(GameObject _gameObjectToAttachDebugger, bool _destroyWhenEmpty)
        {
            if (debugMode) return;

            var debugger = _gameObjectToAttachDebugger.AddComponent<LoadBalancerManagerDebug>();
            debugger.AssignLbm(this, _destroyWhenEmpty);
            debugMode = true;
        }

        public void Tick(float _deltaTime, float _unscaledDeltaTime)
        {
            for (var i = 0; i < loadBalancersList.Count; i++)
            {
                var lb = loadBalancersList[i];
// can lb be null? 
//                if (lb == null) continue;
                var dt = _deltaTime;
                if (lb.UseUnscaledDeltaTime) dt = _unscaledDeltaTime;

#if LB_PERF_DEBUG
                sw.Restart();
#endif
                lb.Tick(dt);
#if LB_PERF_DEBUG

                var elapsedMs = sw.Elapsed.TotalMilliseconds;
                var lbc = loadBalancersDict.ElementAt(i).Key;

                // todo also add debug break option
                if (elapsedMs > LoadBalancerSingleton.logLoadBalancersTakingLongerThanMs) Debug.Log($"{lbc} execution time: {elapsedMs}ms");
#endif
            }
        }

        /// <summary>
        /// Registers using TimeIntervalPerSec type balancer with set frequency
        /// </summary>
        /// <param name="_object"></param>
        /// <param name="_action"></param>
        /// <param name="_tickFrequencyHz"></param>
        /// <param name="_calculateDeltaTime"></param>
        /// <param name="_useUnscaledDt"></param>
        public void Register(object _object, Action<float> _action, int _tickFrequencyHz, bool _calculateDeltaTime = false, bool _useUnscaledDt = false) =>
            Register(_object, _action, new LoadBalancerConfig(LoadBalancerType.XtimesPerSecond, _tickFrequencyHz, _calculateDeltaTime, _useUnscaledDt));

        /// <summary>
        /// Registers using <param name="_loadBalancerConfig"></param>
        /// </summary>
        /// <param name="_object"></param>
        /// <param name="_action"></param>
        /// <param name="_loadBalancerConfig"></param>
        public void Register(object _object, Action<float> _action, LoadBalancerConfig _loadBalancerConfig)
        {
            if (_object == null) return;
            // this is map for every object that maps that object actions to specific lb 

            // get object dictionary for this object or
            if (!objectToActionsMap.TryGetValue(_object, out var objectDictionary))
            {
                // create dictionary for our object if it hasn't yet
                objectDictionary = objectToActionsDictsPool.Count > 0
                    ? objectToActionsDictsPool.Dequeue()
                    : new Dictionary<Action<float>, LoadBalancerConfig>();
                objectToActionsMap.Add(_object, objectDictionary);
            }
            else
                // map new Action<float> to set frequency
            if (objectDictionary.ContainsKey(_action))
            {
                // if this action and this object was already registered, unregister it, that will allow changing call frequency just by calling
                // register again 
                Unregister(_object, _action);
                Register(_object, _action, _loadBalancerConfig);
                return;
            }

            objectDictionary.Add(_action, _loadBalancerConfig);

            // check if we have lb with such frequency
            if (!loadBalancersDict.TryGetValue(_loadBalancerConfig, out var lb))
            {
                lb = _loadBalancerConfig.GetLoadBalancer();
                loadBalancersDict.Add(_loadBalancerConfig, lb);
                loadBalancersList.Add(lb);
            }

            // if we have, add our new Action<float> to it
            lb.AddObject(_action);
        }

        /// <summary>
        /// Removes all actions registered by <paramref name="_object"/>
        /// </summary>
        public bool Unregister(object _object)
        {
            if (_object == null) return false;

            if (!objectToActionsMap.TryGetValue(_object, out var objectDictionary)) return false;

            // loop over all actions of passed object
            foreach (var keyValuePair in objectDictionary)
            {
                loadBalancersDict[keyValuePair.Value].RemoveObject(keyValuePair.Key);
                RemoveLoadBalancerIfEmpty(keyValuePair.Value);
            }

            objectToActionsMap.Remove(_object);
            objectToActionsDictsPool.Enqueue(objectDictionary);
            objectDictionary.Clear();
            return true;
        }

        /// <summary>
        /// Removes only passed _action added by _object
        /// </summary>
        public bool Unregister(object _object, Action<float> _action)
        {
            if (!objectToActionsMap.TryGetValue(_object, out var objectDictionary)) return false;

            var found = false;
            // loop over all actions of passed object
            foreach (var keyValuePair in objectDictionary)
            {
                if (keyValuePair.Key != _action) continue;

                loadBalancersDict[keyValuePair.Value].RemoveObject(keyValuePair.Key);

                RemoveLoadBalancerIfEmpty(keyValuePair.Value);

                var poolableDictionary = objectToActionsMap[_object];

                poolableDictionary.Remove(_action);
                if (poolableDictionary.Count == 0)
                {
                    objectToActionsMap.Remove(_object);
                    objectToActionsDictsPool.Enqueue(poolableDictionary);
                    poolableDictionary.Clear();
//                    poolableDictionary.OnDespawn();
                }

                found = true;
                break;
            }

            return found;
        }

        #endregion

        #region Not public methods

        private void RemoveLoadBalancerIfEmpty(LoadBalancerConfig lbc)
        {
            if (lbc.dontRemoveWhenEmpty) return;

            if (loadBalancersDict[lbc].Actions.Count == 0)
            {
                loadBalancersList.Remove(loadBalancersDict[lbc]);
                loadBalancersDict.Remove(lbc);
            }

            //lbToRemove.Add(lbc);
        }

        #endregion
    }
}