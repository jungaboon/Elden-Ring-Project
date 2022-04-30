// Created by Ronis Vision. All rights reserved
// 20.10.2019.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RVModules.RVUtilities
{
    /// <summary>
    /// todo make thread safe
    /// </summary>
    public class UnityObjectPoolManager : MonoBehaviour
    {
        public RvLogger logger;

        public bool createPoolsOnAwake = true;

        [SerializeField]
        private List<PoolConfig> poolsConfig = new List<PoolConfig>();

        private Dictionary<string, UnityObjectPool> pools = new Dictionary<string, UnityObjectPool>();

        public UnityObjectPool[] GetAllPoolsAsArray => pools.Values.ToArray();
        public Dictionary<string, UnityObjectPool> GetAllPool => pools;

        public bool TryGetPool(string _name, out UnityObjectPool _pool) => pools.TryGetValue(_name, out _pool);

        public bool TryGetPoolByPrefab(GameObject _prefab, out UnityObjectPool _pool)
        {
            foreach (var poolsValue in pools)
            {
                if (poolsValue.Value.Prefab.name != _prefab.name) continue;
                _pool = poolsValue.Value;
                return true;
            }

            _pool = null;
            return false;
        }

        public bool TryGetObject(string _name, out GameObject _gameObject)
        {
            if (pools.TryGetValue(_name, out UnityObjectPool _pool))
            {
                _gameObject = _pool.GetObject();
                return true;
            }

            logger.LogWarning($"There is no pool named {_name}");
            _gameObject = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ReturnObject(GameObject obj)
        {
            if (obj == null) throw new ArgumentNullException();

            if (TryGetPoolByPrefab(obj, out UnityObjectPool _pool))
            {
                _pool.ReturnObject(obj);
                return true;
            }

            logger.LogWarning($"There is no pool for object {obj}", obj);
            return false;
        }

        public void CreatePools()
        {
            if (PoolsCreated)
            {
                logger.LogWarning("Pools has already been created!");
                return;
            }

            var sw = Stopwatch.StartNew();
            foreach (var poolConfig in poolsConfig)
            {
                if (poolConfig.prefab == null)
                {
                    Debug.LogError($"pool {poolConfig.optionalName} prefab cannot be null!");
                    continue;
                }

                var newPool = new UnityObjectPool(poolConfig.prefab, poolConfig.initialSize, poolConfig.allowExpand, poolConfig.optionalParent);
                string poolName = poolConfig.optionalName;
                if (string.IsNullOrEmpty(poolName)) poolName = poolConfig.prefab.name;
                pools.Add(poolName, newPool);
            }

            logger.LogInfo($"{name} pool manager initialization: {sw.ElapsedMilliseconds}ms");
            sw.Stop();
            PoolsCreated = true;
        }

        public bool PoolsCreated { get; private set; }

        private void Awake()
        {
            if (!createPoolsOnAwake) return;
            CreatePools();
        }
    }


    [Serializable] public class PoolConfig
    {
        public string optionalName;
        public GameObject prefab;
        public Transform optionalParent;

        [Range(1, 100)]
        public int initialSize = 20;

        public bool allowExpand = true;
    }
}