// Created by Ronis Vision. All rights reserved
// 23.08.2019.

using UnityEngine;

namespace RVModules.RVSmartAI.Content
{
    /// <summary>
    /// Allows for communication between graph elements and agents' movement logic without coupling them
    /// </summary>
    public interface IMovement
    {
        #region Properties

        /// <summary>
        /// Is agent currently at set destination? Should return true also when no destination was set 
        /// </summary>
        bool AtDestination { get; }

        /// <summary>
        /// Sets current destination and automatically starts moving towards it
        /// Should be set to Vector3.zero after arriving
        /// </summary>
        Vector3 Destination { get; set; }

        /// <summary>
        /// Current velocity
        /// </summary>
        Vector3 Velocity { get; }

        /// <summary>
        /// Get or set movement speed
        /// </summary>
        float MovementSpeed { get; set; }
        
        /// <summary>
        /// Max rotation speed
        /// </summary>
        float RotationSpeed { get; set; }

        /// <summary>
        /// Should movement system take control over agents position? 
        /// </summary>
        bool UpdatePosition { get; set; }

        /// <summary>
        /// Should movement system take control over agents rotation? 
        /// </summary>
        bool UpdateRotation { get; set; }

        /// <summary>
        /// Current position
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// Current rotation
        /// </summary>
        Quaternion Rotation { get; }

        /// <summary>
        /// Agent's main transform
        /// </summary>
        Transform Transform { get; }

        /// <summary>
        /// Create 'blocker' object with collider that is set to destination position
        /// to avoid many agents trying to go to the same position
        /// </summary>
        bool ReserveDestinationPosition { get; set; }

        #endregion
    }
}