// Created by Ronis Vision. All rights reserved
// 05.10.2020.

using System;
using System.Collections.Concurrent;

namespace RVModules.RVUtilities
{
    /// <summary>
    /// Thread safe object pool
    /// </summary>
    public class ObjectPool<T> : IObjectPool<T> where T : IPoolable
    {
        #region Fields

        private ConcurrentQueue<T> objects;
        private Func<T> objectGenerator;

        #endregion

        public ObjectPool(Func<T> _objectGenerator)
        {
            objects = new ConcurrentQueue<T>();
            objectGenerator = _objectGenerator ?? throw new ArgumentNullException(nameof(_objectGenerator));
        }

        #region Public methods

        public T GetObject()
        {
            // this causes gc! Storing it as member var causes other, even bigger problems...
            if (objects.TryDequeue(out var item))
            {
                item.OnSpawn?.Invoke();
                return item;
            }

            item = objectGenerator();
            item.OnDespawn += () => PutObject(item);
            item.OnSpawn?.Invoke();
            return item;
        }

        public void PutObject(T _item) => objects.Enqueue(_item);

        public void Clear() => objects = new ConcurrentQueue<T>();

        #endregion
    }

    public interface IPoolable
    {
        #region Properties

        /// <summary>
        /// Called automatically by ObjectPool
        /// </summary>
        Action OnSpawn { get; set; }

        /// <summary>
        /// Call this when you want to 'destroy'(return to pool) your object 
        /// </summary>
        Action OnDespawn { get; set; }

        #endregion
    }
}