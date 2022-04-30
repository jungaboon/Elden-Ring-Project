// Created by Ronis Vision. All rights reserved
// 11.09.2016.

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RVModules.RVUtilities
{
    /// <summary>
    /// todo make thread-safe - to be safely used from threads
    /// </summary>
    [Serializable]
    public class UnityObjectPool
    {
        #region Fields

        [SerializeField]
        private GameObject parentGO;

        [SerializeField]
        private bool expandable;
        [SerializeField]
        private string name;

        [SerializeField]
        protected GameObject prefab;
    
        private Dictionary<GameObject, IPoolable> usedObjects;
        private List<GameObject> usedObjectsList;
        private Dictionary<GameObject, IPoolable> nonUsedObjects;
        private List<GameObject> nonUsedObjectsList;

        public GameObject Prefab => prefab;

        #endregion

        // constructor
        public UnityObjectPool(GameObject _obj, int _initialPoolSize, bool _expandable, Transform _parent, string _name = "")
        {
            if (!string.IsNullOrEmpty(_name)) name = _name;
            else name = _name;

            prefab = _obj;
            parentGO = new GameObject(_obj.name + "_pool");
            parentGO.transform.parent = _parent;
            usedObjects = new Dictionary<GameObject, IPoolable>();
            nonUsedObjects = new Dictionary<GameObject, IPoolable>();
            usedObjectsList = new List<GameObject>();
            nonUsedObjectsList = new List<GameObject>();
        
            expandable = _expandable;

            ExpandPool(_obj, _initialPoolSize);
        }

        #region Public methods

        public GameObject GetObject()
        {
            GameObject objToReturn = null;

            IPoolable iPoolable = null;

            if (nonUsedObjects.Count == 0 && expandable) ExpandPool(prefab, 1);

            if (nonUsedObjects.Count > 0)
            {
                objToReturn = nonUsedObjectsList[nonUsedObjects.Count-1];
                iPoolable = nonUsedObjects[objToReturn];
            }
            // if all are used, recycle oldest one
            else
            {
                objToReturn = usedObjectsList[0];
                iPoolable = usedObjects[objToReturn];
                ReturnObject(objToReturn);
            }

            //objToReturn = kvp.Key;

            usedObjects.Add(objToReturn, iPoolable);
            usedObjectsList.Add(objToReturn);
        
            nonUsedObjects.Remove(objToReturn);
            nonUsedObjectsList.Remove(objToReturn);

            objToReturn.SetActive(true);

            //kvp.Value?.OnInit();
            iPoolable?.OnSpawn?.Invoke();

            return objToReturn;
        }

        public void ReturnObject(GameObject obj)
        {
            if (!usedObjects.ContainsKey(obj)) return;

            usedObjects[obj]?.OnDespawn();
            obj.SetActive(false);

            nonUsedObjects.Add(obj, usedObjects[obj]);
            nonUsedObjectsList.Add(obj);
        
            usedObjects.Remove(obj);
            usedObjectsList.Remove(obj);
        }
    
        #endregion

        #region Not public methods

        private void ExpandPool(GameObject _sourceGameObject, int _count)
        {
            for (int i = 0; i < _count; i++)
            {
                Vector3 spawnPos = Vector3.zero;

                GameObject obj = Object.Instantiate(_sourceGameObject, spawnPos, Quaternion.identity);
                obj.name = _sourceGameObject.name;
                IPoolable ispawnable = obj.GetComponent<IPoolable>();
                if (ispawnable != null) ispawnable.OnDespawn?.Invoke();
                obj.transform.parent = parentGO.transform;
                obj.SetActive(false);
                nonUsedObjects.Add(obj, ispawnable);
                nonUsedObjectsList.Add(obj);
            }
        }

        #endregion
    }
}