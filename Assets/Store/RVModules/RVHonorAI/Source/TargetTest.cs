// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVHonorAI.Combat;
using RVModules.RVSmartAI.Content.Scanners;
using UnityEngine;

namespace RVHonorAI
{
    /// <summary>
    /// Minimal implementation component that can be detected and attacked by AI
    /// </summary>
    public class TargetTest : MonoBehaviour, ITarget, IRelationship, IScannable
    {
        #region Fields

        [SerializeField]
        private AiGroup aiGroup;

        [SerializeField]
        private Transform visibilityCheckTransform;

        [SerializeField]
        private Transform aimTransform;

        [SerializeField]
        private float danger;

        [SerializeField]
        private float radius;

        #endregion

        #region Properties

        public Transform VisibilityCheckTransform => visibilityCheckTransform;

        public float Radius => radius;

        public Transform Transform => transform;

        public Transform AimTransform => aimTransform;

        public float Danger => danger;

        public AiGroup AiGroup
        {
            get => aiGroup;
            set => aiGroup = value;
        }

        public bool TreatNeutralCharactersAsEnemies { get; }

        #endregion

        #region Public methods

        public bool IsEnemy(IRelationship _other, bool _contraCheck = false) => true;

        public bool IsAlly(IRelationship _other) => false;

        #endregion
    }
}