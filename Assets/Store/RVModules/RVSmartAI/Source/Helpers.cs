// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RVModules.RVSmartAI
{
    public static class Helpers
    {
        #region Public methods

        public static Action<object, object> BuildPropertySetter(object _obj, string _propName)
        {
            var propertyInfo = _obj.GetType().GetProperty(_propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (propertyInfo == null)
            {
                var ex = new Exception($"Couldn't get property '{_propName}' on object '{_obj.GetType()}'");
                Debug.LogException(ex, _obj as Object);
                return null;
            }

            var mi = propertyInfo.GetSetMethod();
            if (mi == null) Debug.LogError($"Couldn't get property setter! Make sure property {_propName} has public setter", _obj as Object);

#if ENABLE_IL2CPP
            // if JIT is not available use slow reflection-based property accessors, because slow is still better than not working at all
            return (_o, _o2) => { mi.Invoke(_o, new[] {_o2}); };
#else
            // if JIT is available use much faster dynamically generated delegates  
//            var fp = new FastProperty(propertyInfo);
//            fp.BuildSetter();
//            return fp.setDelegate;

            // TODO fix fast property setter, for now we use fallback reflection method
            return (_o, _o2) => { mi.Invoke(_o, new[] {_o2}); };
#endif
        }

        public static Func<object, object> BuildPropertyGetter(object _obj, string _propName)
        {
            var propertyInfo = _obj.GetType().GetProperty(_propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (propertyInfo == null)
            {
                var ex = new Exception($"Couldn't get property '{_propName}' on object '{_obj.GetType()}'");
                Debug.LogException(ex, _obj as Object);
                return null;
            }

            var mi = propertyInfo.GetGetMethod();
            if (mi == null) Debug.LogError($"Couldn't get property getter! Make sure property {_propName} has public getter", _obj as Object);

#if ENABLE_IL2CPP
            // if JIT is not available use slow reflection-based property accessors, because slow is still better than not working at all
            return _o => mi.Invoke(_o, null);
#else
            // if JIT is available use much faster dynamically generated delegates  
            var fp = new FastProperty(propertyInfo);
            fp.BuildGetter();
            return fp.getDelegate;
#endif
        }

        // TODO write FastMethod
        public static Action<object> BuildCallMethodAction(object _obj, string _methodName)
        {
            var methodInfo = _obj.GetType().GetMethod(_methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (methodInfo == null)
            {
                var ex = new Exception($"Couldn't get method '{_methodName}' on object '{_obj.GetType()}'");
                Debug.LogException(ex, _obj as Object);
                return null;
            }

#if ENABLE_IL2CPP
            // if JIT is not available use slow reflection-based property accessors, because slow is still better than not working at all
            return (_o) => { methodInfo.Invoke(_o, null); };
#else
            var fm = new FastMethod(methodInfo);
            return fm.callDelegate;
#endif
        }

        #endregion
    }

    public class FastProperty
    {
        #region Fields

        public Func<object, object> getDelegate;

        public Action<object, object> setDelegate;

        #endregion

        #region Properties

        public PropertyInfo Property { get; }

        #endregion

        public FastProperty(PropertyInfo _property) => Property = _property;

        #region Public methods

        public void BuildSetter() => InitializeSet();

        public void BuildGetter() => InitializeGet();

        public object Get(object _instance) => getDelegate(_instance);

        public void Set(object _instance, object _value) => setDelegate(_instance, _value);

        #endregion

        #region Not public methods

        private void InitializeSet()
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var value = Expression.Parameter(typeof(object), "value");

            // value as T is slightly faster than (T)value, so if it's not a value type, use that

            var instanceCast = !Property.DeclaringType.IsValueType
                ? Expression.TypeAs(instance, Property.DeclaringType)
                : Expression.Convert(instance, Property.DeclaringType);

            var valueCast = !Property.PropertyType.IsValueType
                ? Expression.TypeAs(value, Property.PropertyType)
                : Expression.Convert(value, Property.PropertyType);

            setDelegate = Expression.Lambda<Action<object, object>>(Expression.Call(instanceCast, Property.GetSetMethod(), valueCast),
                instance, value).Compile();
        }

        private void InitializeGet()
        {
            var instance = Expression.Parameter(typeof(object), "instance");

            var instanceCast = !Property.DeclaringType.IsValueType
                ? Expression.TypeAs(instance, Property.DeclaringType)
                : Expression.Convert(instance, Property.DeclaringType);

            getDelegate = Expression
                .Lambda<Func<object, object>>(Expression.TypeAs(Expression.Call(instanceCast, Property.GetGetMethod()), typeof(object)), instance)
                .Compile();
        }

        #endregion
    }

    public class FastMethod
    {
        #region Fields

        public Action<object> callDelegate;

        #endregion

        #region Properties

        public MethodInfo Method { get; }

        #endregion

        public FastMethod(MethodInfo _property)
        {
            Method = _property;
            BuildFastMethodCall();
        }

        #region Not public methods

        private void BuildFastMethodCall()
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            //var value = Expression.Parameter(typeof(object), "value");

            // value as T is slightly faster than (T)value, so if it's not a value type, use that

            var instanceCast = !Method.DeclaringType.IsValueType
                ? Expression.TypeAs(instance, Method.DeclaringType)
                : Expression.Convert(instance, Method.DeclaringType);

//            var valueCast = !Method.PropertyType.IsValueType
//                ? Expression.TypeAs(value, Method.PropertyType)
//                : Expression.Convert(value, Method.PropertyType);

            callDelegate = Expression.Lambda<Action<object>>(Expression.Call(instanceCast, Method), instance).Compile();
        }

        #endregion
    }
}