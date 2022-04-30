// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using System;
using System.Collections.Generic;

namespace RVModules.RVUtilities
{
    /// <summary>
    /// Allows you to store one instance of object per type
    /// </summary>
    public class DataContainer
    {
        #region Fields

        private static DataContainer instance;

        private List<object> datas = new List<object>();
        private Dictionary<Type, object> dataDict = new Dictionary<Type, object>();

        #endregion

        #region Properties

        public static DataContainer Instance
        {
            get
            {
                if (instance == null)
                    instance = new DataContainer();
                return instance;
            }
        }

        #endregion

        #region Public methods

        public void ResetAll()
        {
            datas.Clear();
            dataDict.Clear();
        }

        public void AddData<T>(T _data)
        {
            if (dataDict.TryGetValue(typeof(T), out var data))
            {
                dataDict[typeof(T)] = _data;
                return;
            }

            dataDict.Add(typeof(T), _data);
            datas.Add(_data);
        }

        public T GetData<T>() where T : class
        {
            object data;
            if (!dataDict.TryGetValue(typeof(T), out data))
                return null;

            var castedData = data as T;
            return castedData;
        }

        public void RemoveData(object _data)
        {
            foreach (var data in datas)
            {
                if (!_data.Equals(data)) continue;
                datas.Remove(data);
                dataDict.Remove(data.GetType());
                return;
            }
        }

        #endregion
    }
}