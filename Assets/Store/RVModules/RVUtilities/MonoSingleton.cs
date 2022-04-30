// Created by Ronis Vision. All rights reserved
// 09.08.2020.

using UnityEngine;

namespace RVModules.RVUtilities
{
    /// <summary>
    /// MonoBehaviour based singleton implementation
    /// </summary>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        #region Fields

        protected static T instance;
        private static bool initialized;

        private new string name;

        [SerializeField]
        protected bool isSingleton = true;

        [SerializeField]
        [Tooltip("This requires to unparent transform this singleton is on")]
        private bool dontDestroy = true;

        #endregion

        #region Properties

        /// <summary>
        /// Name for singleton game object. Default use GetType().Name
        /// </summary>
        public virtual string Name
        {
            get => string.IsNullOrEmpty(name) ? GetType().Name : name;
            protected set => name = value;
        }

        /// <summary>
        /// Make sure to check for null after adressing Instance as it can return null, if singleton carrier game object was destroyed
        /// typically its on exiting from play mode in editor.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (initialized) return instance;
                //if (instance != null) return instance;

                GameObject go = null;
                if (instance == null)
                {
                    var all = FindObjectsOfType<T>();
                    foreach (var monoSingleton in all)
                        if (monoSingleton.isSingleton)
                            instance = monoSingleton;

                    if (instance == null)
                    {
                        go = new GameObject();
                        instance = go.AddComponent<T>();
                    }
                    else go = instance.gameObject;
                }
                else
                {
                    go = instance.gameObject;
                }

                if (instance.dontDestroy)
                {
                    instance.transform.SetParent(null);
                    DontDestroyOnLoad(go);
                }

                instance.SingletonInitialization();
                go.name = instance.Name;
                initialized = true;
                return instance;
            }
        }

        #endregion

        #region Public methods

        public static void DestroySingleton()
        {
            var instanc = Instance;
            var go = Instance.gameObject;
            initialized = false;
            DestroyImmediate(instanc);
            DestroyImmediate(go);
        }

        #endregion

        #region Not public methods

        /// <summary>
        /// Called after singleton creation (in Awake or first Instance call)
        /// </summary>
        protected virtual void SingletonInitialization()
        {
        }

        /// <summary>
        /// Initializes singleton if InitializeSingletonOnAwake is true
        /// </summary>
        protected virtual void Awake()
        {
            if (isSingleton)
            {
                instance = GetComponent<T>();
                var singleton = Instance;
            }
        }

        #endregion
    }
}