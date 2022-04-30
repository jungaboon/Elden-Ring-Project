// Created by Ronis Vision. All rights reserved
// 07.08.2020.

using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RVModules.RVCommonGameLibrary.Pooling
{
    [Serializable] public class PoolConfig
    {
        #region Fields

        /// <summary>
        /// Use this if you don't want to use prefab's name
        /// </summary>
        public string optionalName;
        
        public GameObject prefab;

        /// <summary>
        /// Optional parent object, spwaned objects will be parented automatically as its children
        /// </summary>
        public Transform spawnParent;

        public MultiplePoolableHandling multiplePoolableHandling;

        /// <summary>
        /// Returns prefab's name or optionalName if not null or empty
        /// </summary>
        public string PoolName => string.IsNullOrEmpty(optionalName) ? prefab.name : optionalName;

        public PoolConfig()
        {
        }

        public PoolConfig(GameObject _prefab)
        {
            if(_prefab == null) throw new NullReferenceException("Can't create pool with null prefab");
            prefab = _prefab;
        }

        public PoolConfig(GameObject _prefab, Transform _spawnParent = null, MultiplePoolableHandling _multiplePoolableHandling = MultiplePoolableHandling.None)
        {
            if(_prefab == null) throw new NullReferenceException("Can't create pool with null prefab");
            prefab = _prefab;
            spawnParent = _spawnParent;
            multiplePoolableHandling = _multiplePoolableHandling;
        }

        public PoolConfig(GameObject _prefab, string _poolName, Transform _spawnParent = null,
            MultiplePoolableHandling _multiplePoolableHandling = MultiplePoolableHandling.None)
        {
            if(_prefab == null) throw new NullReferenceException("Can't create pool with null prefab");
            optionalName = _poolName;
            prefab = _prefab;
            spawnParent = _spawnParent;
            multiplePoolableHandling = _multiplePoolableHandling;
        }

        #endregion
    }

    public enum MultiplePoolableHandling
    {
        None,
        GetComponents,
        GetComponentsInChildren
    }
}