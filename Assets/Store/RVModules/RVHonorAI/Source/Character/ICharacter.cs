// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVHonorAI.Combat;
using RVModules.RVSmartAI.Content.Scanners;
using UnityEngine;
using UnityEngine.Events;

namespace RVHonorAI
{
    /// <summary>
    /// The most general character-defining contract 
    /// todo to consider if it would be good idea to have two - different lvl of detail interfaces of ai agents - like IAiAgent and ICharacter,
    /// with IAiAgent being very lightweight(like Ichar now) and ICharacter that would be much more HonorAi systems-specific 
    /// </summary>
    public interface ICharacter : IHitPoints, ITarget, IScannable, IDamageable, IRelationship, ITargetProvider
    {
        #region Properties

        /// <summary>
        /// UnityEvent called when character dies
        /// </summary>
        UnityEvent OnKilled { get; set; }

        /// <summary>
        /// Running speed, in m/s
        /// </summary>
        float RunningSpeed { get; set; }

        /// <summary>
        /// Walking speed, in m/s
        /// </summary>
        float WalkingSpeed { get; set; }

        IAttack CurrentAttack { get; }

        /// <summary>
        /// Used weapon
        /// </summary>
        IWeapon CurrentWeapon { get; }

        ICharacterAi CharacterAi { get; }

        #endregion

        #region Public methods

        /// <summary>
        /// Instantly kills this character, plays die sound and creates ragdoll or play killed animation depending on configuration 
        /// </summary>
        void Kill();

        /// <summary>
        /// Instantly kills this character, plays die sound and creates ragdoll or play killed animation depending on configuration 
        /// </summary>
        void Kill(Vector3 hitPoint, Vector3 hitForce = default, float forceRadius = default);

        #endregion


//        Transform HeadTransform { get; }
//        float Health { get; }
//        AiGroup AiGroup { get; }
//        float DamagePerSecond { get; }
//        float Armor { get; }
//        float Courage { get; }
//        float AttackRange { get; }
    }
}