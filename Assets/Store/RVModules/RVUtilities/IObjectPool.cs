// Created by Ronis Vision. All rights reserved
// 03.07.2021.

namespace RVModules.RVUtilities
{
    /// <summary>
    /// ObjectPool interface
    /// </summary>
    /// <typeparam name="T">Type of object, must implement IPoolable</typeparam>
    public interface IObjectPool<T> where T : IPoolable
    {
        T GetObject();
        void PutObject(T _item);
        void Clear();
    }
}