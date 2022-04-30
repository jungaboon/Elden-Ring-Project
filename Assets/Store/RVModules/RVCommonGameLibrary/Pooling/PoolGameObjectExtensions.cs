// Created by Ronis Vision. All rights reserved
// 10.02.2021.

using RVModules.RVUtilities;
using UnityEngine;

namespace RVModules.RVCommonGameLibrary.Pooling
{
    public static class PoolGameObjectExtensions
    {
        /// <summary>
        /// Short for PoolManager.Instance.TryGetObject that assumes pool for this prefab exists
        /// </summary>
        public static T GetFromPool<T>(this GameObject _prefab) where T : IPoolable
        {
            PoolManager.Instance.TryGetObject(_prefab.name, out T _poolable);
            return _poolable;
        }

        /// <summary>
        /// Short for PoolManager.Instance.TryGetObject that assumes pool for this prefab exists
        /// </summary>
        public static T GetFromPool<T>(this T _poolableComponent) where T : IPoolable
        {
            var comp = _poolableComponent as Component;
            if (comp == null)
            {
                Debug.LogError("IPoolable must be of Component type!");
                return default;
            }

            PoolManager.Instance.TryGetObject(comp.gameObject.name, out T _poolable);
            return _poolable;
        }

        /// <summary>
        /// Short for PoolManager.Instance.TryGetObject
        /// </summary>
        public static bool TryGetFromPool<T>(this GameObject _prefab, out T _poolable) where T : IPoolable
        {
            return PoolManager.Instance.TryGetObject(_prefab.name, out _poolable);
        }

        /// <summary>
        /// Short for PoolManager.Instance.CreatePoolIfDoesntExist
        /// </summary>
        public static void CreatePoolIfDoesntExist(this GameObject _prefab, PoolConfig _poolConfig = null)
        {
            PoolManager.Instance.CreatePoolIfDoesntExist(_poolConfig ?? new PoolConfig(_prefab));
        }
    }
}