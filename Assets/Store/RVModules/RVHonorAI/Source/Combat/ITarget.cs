// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI;
using UnityEngine;

namespace RVHonorAI.Combat
{
    /// <summary>
    /// General Target contract. ITarget doesn't define type of target, it can be anything
    /// </summary>
    public interface ITarget : IComponent, IVisibilityCheckTransformProvider, IDangerProvider
    {
        #region Properties

        /// <summary>
        /// Target's radius, used to calculate distance
        /// </summary>
        float Radius { get; }

        /// <summary>
        /// Target's main transform, for checking position, rotation etc.. usually root transform of entity
        /// </summary>
        Transform Transform { get; }

        /// <summary>
        /// Transform for aiming at target, also used for checking fov with raycasts etc...
        /// </summary>
        Transform AimTransform { get; }

        /// <summary>
        /// Target's danger
        /// It may depend on many factors, like distance, firepower, strength, health etc...
        /// </summary>
        new float Danger { get; }

        #endregion
    }
}