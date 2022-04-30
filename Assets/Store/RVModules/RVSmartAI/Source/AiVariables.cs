// Created by Ronis Vision. All rights reserved
// 25.01.2021.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RVModules.RVSmartAI
{
    /// <summary>
    /// Graph variables. Allows to store many different types of variables and reference them via (string)name.  
    /// Can help avoid implementing many often used variables like float, animation curve etc in context object and writing dedicated graph
    /// elements to access them - DataProviders have 'variable' version for every supported type that gets data from graph variables(eg VariableFloatProvider).
    /// Also very useful for data shared between graph elements.
    /// For all primitive types you can check if it exist (eg HasBool), get variable (eg GetBool) or set variable (eg SetBool).
    /// UnityObjects and Objects variables can also be added via api using AddObjectVariable and AddUnityObjectVariable.
    /// UnityObjects variables will store all sceneReferences from Ai component it is assigned to at runtime - this is mostly useful for easy access to
    /// position of scene game object in any graph element.
    ///
    /// todo lists for all already existing vars
    /// </summary>
    public class AiVariables : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private GraphVariableBool[] bools;

        [SerializeField]
        private GraphVariableInt[] ints;

        [SerializeField]
        private GraphVariableFloat[] floats;

        [SerializeField]
        private GraphVariableString[] strings;

        [SerializeField]
        private GraphVariableVector2[] vectors2;

        [SerializeField]
        private GraphVariableVector3[] vectors3;

        [SerializeField]
        private GraphVariableVector3List[] vectors3Lists;

        [SerializeField]
        private GraphVariableLayerMask[] layerMasks;

        [SerializeField]
        private GraphVariableAnimationCurve[] animationCurves;

        [SerializeField]
        private List<GraphVariableUnityObject> unityObjects = new List<GraphVariableUnityObject>(10);

        private Dictionary<string, bool> boolsDictionary = new Dictionary<string, bool>();
        private Dictionary<string, int> intsDictionary = new Dictionary<string, int>();
        private Dictionary<string, float> floatsDictionary = new Dictionary<string, float>();
        private Dictionary<string, string> stringsDictionary = new Dictionary<string, string>();
        private Dictionary<string, Vector2> vector2sDictionary = new Dictionary<string, Vector2>();
        private Dictionary<string, Vector3> vector3sDictionary = new Dictionary<string, Vector3>();
        private Dictionary<string, List<Vector3>> vector3ListsDictionary = new Dictionary<string, List<Vector3>>();
        private Dictionary<string, LayerMask> layerMasksDictionary = new Dictionary<string, LayerMask>();
        private Dictionary<string, AnimationCurve> animationCurvesDictionary = new Dictionary<string, AnimationCurve>();
        private Dictionary<string, Object> unityObjectsDictionary = new Dictionary<string, Object>();
        private Dictionary<string, object> objectsDictionary = new Dictionary<string, object>();

        #endregion

        #region Public methods

        /// <summary>
        /// for inspector debug only
        /// </summary>
        /// <returns></returns>
        public string[] GetAllVariablesAsStrings()
        {
            var vars = GetAllVarsToStrings(boolsDictionary, intsDictionary, floatsDictionary, stringsDictionary, vector2sDictionary, vector3sDictionary,
                vector3ListsDictionary, layerMasksDictionary, animationCurvesDictionary, unityObjectsDictionary, objectsDictionary);
            return vars;
        }

        public bool HasBool(string _name) => boolsDictionary.ContainsKey(_name);
        public bool HasInt(string _name) => intsDictionary.ContainsKey(_name);
        public bool HasFloat(string _name) => floatsDictionary.ContainsKey(_name);
        public bool HasString(string _name) => stringsDictionary.ContainsKey(_name);
        public bool HasVector2(string _name) => vector2sDictionary.ContainsKey(_name);
        public bool HasVector3(string _name) => vector3sDictionary.ContainsKey(_name);
        public bool HasLayerMask(string _name) => layerMasksDictionary.ContainsKey(_name);
        public bool HasAnimationCurve(string _name) => animationCurvesDictionary.ContainsKey(_name);
        public bool HasUnityObject(string _name) => unityObjectsDictionary.ContainsKey(_name);
        public bool HasObject(string _name) => objectsDictionary.ContainsKey(_name);


        /// <summary>
        /// Checks if there is variable named <paramref name="_name"/>, if it doesn't it adds one with default value, does nothing otherwise
        /// </summary>
        /// <param name="_name">Name of variable</param>
        /// <returns>True if variable already existed, false if it had to be added</returns>
        public bool AssureBoolExist(string _name) => AssureVarExist<bool>(_name, boolsDictionary);

        /// <summary>
        /// Checks if there is variable named <paramref name="_name"/>, if it doesn't it adds one with default value, does nothing otherwise
        /// </summary>
        /// <param name="_name">Name of variable</param>
        /// <returns>True if variable already existed, false if it had to be added</returns>
        public bool AssureIntExist(string _name) => AssureVarExist<int>(_name, intsDictionary);

        /// <summary>
        /// Checks if there is variable named <paramref name="_name"/>, if it doesn't it adds one with default value, does nothing otherwise
        /// </summary>
        /// <param name="_name">Name of variable</param>
        /// <returns>True if variable already existed, false if it had to be added</returns>
        public bool AssureFloatExist(string _name) => AssureVarExist<float>(_name, floatsDictionary);

        /// <summary>
        /// Checks if there is variable named <paramref name="_name"/>, if it doesn't it adds one with default value, does nothing otherwise
        /// </summary>
        /// <param name="_name">Name of variable</param>
        /// <returns>True if variable already existed, false if it had to be added</returns>
        public bool AssureStringExist(string _name) => AssureVarExist<string>(_name, stringsDictionary);

        /// <summary>
        /// Checks if there is variable named <paramref name="_name"/>, if it doesn't it adds one with default value, does nothing otherwise
        /// </summary>
        /// <param name="_name">Name of variable</param>
        /// <returns>True if variable already existed, false if it had to be added</returns>
        public bool AssureVector2Exist(string _name) => AssureVarExist<Vector2>(_name, vector2sDictionary);

        /// <summary>
        /// Checks if there is variable named <paramref name="_name"/>, if it doesn't it adds one with default value, does nothing otherwise
        /// </summary>
        /// <param name="_name">Name of variable</param>
        /// <returns>True if variable already existed, false if it had to be added</returns>
        public bool AssureVector3Exist(string _name) => AssureVarExist<Vector3>(_name, vector3sDictionary);

        /// <summary>
        /// Checks if there is variable named <paramref name="_name"/>, if it doesn't it adds one with default value, does nothing otherwise
        /// </summary>
        /// <param name="_name">Name of variable</param>
        /// <returns>True if variable already existed, false if it had to be added</returns>
        public bool AssureVector3ListExist(string _name) => AssureVarExist<List<Vector3>>(_name, vector3ListsDictionary);

        /// <summary>
        /// Checks if there is variable named <paramref name="_name"/>, if it doesn't it adds one with default value, does nothing otherwise
        /// </summary>
        /// <param name="_name">Name of variable</param>
        /// <returns>True if variable already existed, false if it had to be added</returns>
        public bool AssureLayerMaskExist(string _name) => AssureVarExist<LayerMask>(_name, layerMasksDictionary);

        /// <summary>
        /// Checks if there is variable named <paramref name="_name"/>, if it doesn't it adds one with default value, does nothing otherwise
        /// </summary>
        /// <param name="_name">Name of variable</param>
        /// <returns>True if variable already existed, false if it had to be added</returns>
        public bool AssureAnimationCurveExist(string _name) => AssureVarExist<AnimationCurve>(_name, animationCurvesDictionary);

        /// <summary>
        /// Checks if there is variable named <paramref name="_name"/>, if it doesn't it adds one with default value, does nothing otherwise
        /// </summary>
        /// <param name="_name">Name of variable</param>
        /// <returns>True if variable already existed, false if it had to be added</returns>
        public bool AssureUnityObjectExist(string _name) => AssureVarExist<Object>(_name, unityObjectsDictionary);

        /// <summary>
        /// Checks if there is variable named <paramref name="_name"/>, if it doesn't it adds one with default value, does nothing otherwise
        /// </summary>
        /// <param name="_name">Name of variable</param>
        /// <returns>True if variable already existed, false if it had to be added</returns>
        public bool AssureObjectExist(string _name) => AssureVarExist<string>(_name, objectsDictionary);

        public bool GetBool(string _name) => boolsDictionary[_name];
        public void SetBool(string _name, bool _value) => boolsDictionary[_name] = _value;

        public int GetInt(string _name) => intsDictionary[_name];
        public void SetInt(string _name, int _value) => intsDictionary[_name] = _value;

        public float GetFloat(string _name) => floatsDictionary[_name];
        public void SetFloat(string _name, float _value) => floatsDictionary[_name] = _value;

        public string GetString(string _name) => stringsDictionary[_name];
        public void SetString(string _name, string _value) => stringsDictionary[_name] = _value;

        public Vector2 GetVector2(string _name) => vector2sDictionary[_name];
        public void SetVector2(string _name, Vector2 _value) => vector2sDictionary[_name] = _value;

        public Vector3 GetVector3(string _name) => vector3sDictionary[_name];
        public void SetVector3(string _name, Vector3 _value) => vector3sDictionary[_name] = _value;

        public List<Vector3> GetVector3List(string _name) => vector3ListsDictionary[_name];
        public void SetVector3List(string _name, List<Vector3> _value) => vector3ListsDictionary[_name] = _value;

        public LayerMask GetLayerMask(string _name) => layerMasksDictionary[_name];
        public void SetLayerMask(string _name, LayerMask _value) => layerMasksDictionary[_name] = _value;

        public AnimationCurve GetAnimationCurve(string _name) => animationCurvesDictionary[_name];
        public void SetAnimationCurve(string _name, AnimationCurve _value) => animationCurvesDictionary[_name] = _value;

        public Object GetUnityObject(string _name) => unityObjectsDictionary[_name];
        public T GetUnityObjectAs<T>(string _name) where T : class => unityObjectsDictionary[_name] as T;
        public void SetUnityObject(string _name, Object _value) => unityObjectsDictionary[_name] = _value;

        public object GetObject(string _name) => objectsDictionary[_name];
        public T GetObjectAs<T>(string _name) where T : class => objectsDictionary[_name] as T;
        public void SetObject(string _name, object _value) => objectsDictionary[_name] = _value;

        public void AddUnityObjectVariable(string _name, Object _unityObject)
        {
            if (unityObjectsDictionary.ContainsKey(_name))
            {
                Debug.LogError($"There is already UnityObject variable named {_name}");
                return;
            }

            unityObjects.Add(new GraphVariableUnityObject(_name, _unityObject));
            unityObjectsDictionary.Add(_name, _unityObject);
        }

        public void AddObjectVariable(string _name, object _variableObject)
        {
            if (objectsDictionary.ContainsKey(_name))
            {
                Debug.LogError($"There is already object variable named {_name}");
                return;
            }

            objectsDictionary.Add(_name, _variableObject);
        }

        #endregion

        #region Not public methods

        private string[] GetAllVarsToStrings(params IDictionary[] _dictionaries)
        {
            var content = new List<string>(_dictionaries.Length * 5);

            foreach (var dictionary in _dictionaries)
            foreach (DictionaryEntry dictionaryEntry in dictionary)
                if (dictionaryEntry.Value is IEnumerable enumerable && !(dictionaryEntry.Value is string))
                {
                    content.Add($"{dictionaryEntry.Key}:");
                    foreach (var v in enumerable) content.Add(v.ToString());
                }
                else
                {
                    content.Add($"{dictionaryEntry.Key}: {dictionaryEntry.Value}");
                }

            return content.ToArray();
        }

        private bool AssureVarExist<T>(string _name, IDictionary _dictionary)
        {
            if (_dictionary.Contains(_name)) return true;
            _dictionary.Add(_name, default(T));
            return false;
        }

        private void Awake()
        {
            InitializeDictionaries();
            InitializeOwnDictionaries();
        }

        protected virtual void InitializeOwnDictionaries()
        {
        }

        private void InitializeDictionaries()
        {
            FillDictionary(bools, boolsDictionary);
            FillDictionary(ints, intsDictionary);
            FillDictionary(floats, floatsDictionary);
            FillDictionary(strings, stringsDictionary);
            FillDictionary(vectors2, vector2sDictionary);
            FillDictionary(vectors3, vector3sDictionary);
            FillDictionary(vectors3Lists, vector3ListsDictionary);
            FillDictionary(layerMasks, layerMasksDictionary);
            FillDictionary(animationCurves, animationCurvesDictionary);
            FillDictionary(unityObjects.ToArray(), unityObjectsDictionary);
        }

        protected void FillDictionary<T>(GraphVariable<T>[] _graphVariables, Dictionary<string, T> _dictionary)
        {
            _dictionary.Clear();
            foreach (var graphVariable in _graphVariables) _dictionary.Add(graphVariable.name, graphVariable.value);
        }

        #endregion


        [Serializable] public class GraphVariableBool : GraphVariable<bool>
        {
        }

        [Serializable] public class GraphVariableInt : GraphVariable<int>
        {
        }

        [Serializable] public class GraphVariableFloat : GraphVariable<float>
        {
        }

        [Serializable] public class GraphVariableString : GraphVariable<string>
        {
        }

        [Serializable] public class GraphVariableVector3 : GraphVariable<Vector3>
        {
        }

        [Serializable] public class GraphVariableVector3List : GraphVariable<List<Vector3>>
        {
        }

        [Serializable] public class GraphVariableVector2 : GraphVariable<Vector2>
        {
        }

        [Serializable] public class GraphVariableAnimationCurve : GraphVariable<AnimationCurve>
        {
        }

        [Serializable] public class GraphVariableLayerMask : GraphVariable<LayerMask>
        {
        }

        [Serializable] public class GraphVariableUnityObject : GraphVariable<Object>
        {
            public GraphVariableUnityObject(string _name, Object _value) : base(_name, _value)
            {
            }
        }

        [Serializable] public class GraphVariableObject : GraphVariable<object>
        {
        }

        public class GraphVariable<T>
        {
            #region Fields

            public string name;
            public T value;

            #endregion

            protected GraphVariable(string _name, T _value)
            {
                name = _name;
                value = _value;
            }

            protected GraphVariable()
            {
            }
        }
    }
}