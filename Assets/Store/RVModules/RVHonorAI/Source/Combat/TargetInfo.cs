// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using System;
using RVModules.RVSmartAI;
using RVModules.RVUtilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RVHonorAI.Combat
{
    /// <summary>
    /// Used to store informations about target like last seen position, is it currently visible etc
    /// </summary>
    [Serializable] public class TargetInfo : IPoolable, IUnityComponent, IConvertibleParam<Vector3>
    {
        #region Fields

        /// <summary>
        /// target casted on Unity object, for debug(inspector) only, editor only(wont be set in build)
        /// </summary>
        [SerializeField]
        private Object targetObject;

        [SerializeField]
        private Vector3 lastSeenPosition;

        [SerializeField]
        private float lastSeenTime;

        [SerializeField]
        private bool visible;

        [SerializeField]
        [Tooltip("For debugging only")]
        private float danger;

        private ITarget target;

        #endregion

        #region Properties

        public Action OnSpawn { get; set; }
        public Action OnDespawn { get; set; }

        public ITarget Target
        {
            get => target;
            set
            {
                target = value;
#if UNITY_EDITOR
                if (target.Object() != null) danger = target.Danger;
                targetObject = target.Object();
#endif
            }
        }

        public Vector3 LastSeenPosition
        {
            get => lastSeenPosition;
            set => lastSeenPosition = value;
        }

        public float LastSeenTime
        {
            get => lastSeenTime;
            set => lastSeenTime = value;
        }

        public bool Visible
        {
            get => visible;
            set => visible = value;
        }

        public Type MyType { get; }

        #endregion

        public TargetInfo()
        {
            MyType = GetType();
            OnSpawn += () =>
            {
                Target = null;
                danger = 0;
#if UNITY_EDITOR
                targetObject = null;
#endif
                LastSeenPosition = Vector3.zero;
                LastSeenTime = 0;
                Visible = false;
            };
        }

        /// <summary>
        /// Constructor for new ITarget
        /// </summary>
        /// <param name="_target">Target</param>
        /// <param name="_lastSeenPosition">If left unchanged, will be set to target's transform position</param>
        /// <param name="_visible">Should target be immediately visible</param>
        public TargetInfo(ITarget _target, Vector3 _lastSeenPosition = default, bool _visible = true)
        {
            lastSeenPosition = _lastSeenPosition;
            if (lastSeenPosition == default) lastSeenPosition = _target.Transform.position;
            visible = _visible;
            target = _target;
        }

        #region Public methods

        public Component ToUnityComponent() => targetObject as Component;

        public static implicit operator Vector3(TargetInfo _targetInfo) => _targetInfo.LastSeenPosition;

        public Vector3 Convert() => Target.Transform.position;

        #endregion
    }

//    public class TargetInfoToVector3Converted : TypeConverter
//    {
//        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
//        {
//            return destinationType == typeof(Vector3) || base.CanConvertTo(context, destinationType);
//        }
//
//        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
//        {
//            if (destinationType == typeof(Vector3)) return (Vector3) (TargetInfo) value;
//            return base.ConvertTo(context, culture, value, destinationType);
//        }
//    }
}