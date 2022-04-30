// Created by Ronis Vision. All rights reserved
// 03.07.2021.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RVModules.RVUtilities
{
    /// <summary>
    /// Just like with ObjectPool, but works on list instead of ConcurrentQueue to allow for removing specific elements from pool
    /// This is needed to handle Unity game object that can be destroyed at any time, so specific elements have to be removed from pool,
    /// which is impossible in case of normal ObjectPool
    /// </summary>
    public class ObjectPoolRemovable<T> : IObjectPool<T> where T : IPoolable
    {
        #region Fields

        private List<T> objects;
        private Func<T> objectGenerator;

        #endregion

        public ObjectPoolRemovable(Func<T> _objectGenerator)
        {
            objects = new List<T>();
            objectGenerator = _objectGenerator ?? throw new ArgumentNullException(nameof(_objectGenerator));
        }

        #region Public methods

        public T GetObject()
        {
            if (objects.Count > 0)
            {
                var item = objects[objects.Count - 1];
                objects.Remove(item);
                item.OnSpawn?.Invoke();
                return item;
            }
            // if (objects.TryDequeue(out var item))
            // {
            //     item.OnSpawn?.Invoke();
            //     return item;
            // }

            var createdItem = objectGenerator();
            createdItem.OnDespawn += () => PutObject(createdItem);
            createdItem.OnSpawn?.Invoke();
            return createdItem;
        }

        public void PutObject(T _item) => objects.Add(_item);

        public void Clear() => objects = new List<T>();

        public void RemoveFromPool(T _poolable)
        {
            objects.Remove(_poolable);
        }

        #endregion
    }
}