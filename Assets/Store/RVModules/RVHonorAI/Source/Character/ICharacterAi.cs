// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using System.Collections.Generic;
using RVHonorAI.Combat;
using RVModules.RVSmartAI;
using RVModules.RVSmartAI.Content.AI.Contexts;
using UnityEngine;

namespace RVHonorAI
{
    /// <summary>
    /// todo description
    /// </summary>
    public interface ICharacterAi : IMovementProvider, IMovementScannerProvider, IEnvironmentScannerProvider, IMoveTargetProvider, IWaypointsProvider,
        INearbyObjectsProvider, ICharacterProvider, ITargetProvider, IFovMaskProvider, IDetectionRangeProvider, IFovPositionProvider, ICharacterState,
        IMovementSpeedProvider, ICharacterAnimationProvider, ICourageProvider, ITargetInfosProvider, IRelationship
    {
        #region Properties

        /// <summary>
        /// Head transform. Used for checking fov, look at etc...
        /// </summary>
        Transform HeadTransform { get; set; }

        /// <summary>
        /// Unity's enabled wrapper
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Ai component
        /// </summary>
        Ai Ai { get; }

        /// <summary>
        /// Character's waypoints
        /// </summary>
        List<Waypoint> Waypoints { get; }

        /// <summary>
        /// Add new target to targets list. Whether it will be targeted or not is up to AI target selection logic
        /// </summary>
        /// <param name="_target">Target to add</param>
        /// <param name="_visible">Should this target be immediately marked as visible</param>
        /// <param name="_lastSeenPosition">Last seen pos will be set to this if not default</param>
        void AddTarget(ITarget _target, bool _visible = true, Vector3 _lastSeenPosition = default);

        /// <summary>
        /// Immediately targets provided ITarget
        /// </summary>
        /// <param name="_target">Target object</param>
        /// <param name="_visible">Should this target be immediately marked as visible</param>
        /// <param name="_lastSeenPosition">Last seen pos will be set to this if not default</param>
        void SetTarget(ITarget _target, bool _visible = true, Vector3 _lastSeenPosition = default);

        #endregion
    }
}