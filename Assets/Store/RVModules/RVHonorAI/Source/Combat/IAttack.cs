// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVCommonGameLibrary.Audio;
using UnityEngine;

namespace RVHonorAI.Combat
{
    /// <summary>
    /// Attack definition
    /// </summary>
    public interface IAttack
    {
        #region Properties
        
        /// <summary>
        /// If false, damage will be also applied to neutral and friendly characters
        /// </summary>
        bool DamageOnlyEnemies { get; }

        /// <summary>
        /// Attack name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        AnimationClip AnimationClip { get; set; }

        /// <summary>
        /// Weapon of this attack
        /// </summary>
        IWeapon Weapon { get; }

        /// <summary>
        /// Damage type
        /// </summary>
        DamageType DamageType { get; }

        /// <summary>
        /// Damage
        /// </summary>
        float Damage { get; }

        /// <summary>
        /// 
        /// </summary>
        float DamagePerSecond { get; }

        /// <summary>
        /// How long after attack started will deal damage
        /// Used only if not using animation attack events
        /// </summary>
        float DamageTime { get; }

        /// <summary>
        /// Overall multiplier for chance of selecting attack
        /// </summary>
        float Preference { get; }

        /// <summary>
        /// Maximum range in which target will still be hit
        /// </summary>
        float DamageRange { get; }

        /// <summary>
        /// Maximum range in which this attack should be commenced to still hit target
        /// It should take into consideration AttackTranslation 
        /// </summary>
        float EngageRange { get; }

        /// <summary>
        /// Preferred engage range
        /// </summary>
        float PreferredEngageRange { get; }

        /// <summary>
        /// How long this attack take to execute, this is total attack time, not just to dealing damage moment
        /// </summary>
        float AttackDuration { get; }

        /// <summary>
        /// Attack angle in degrees on Y axis
        /// </summary>
        float AttackAngle { get; }

        /// <summary>
        /// Attak type
        /// </summary>
        AttackType AttackType { get; }

        /// <summary>
        /// Attack sound override, can be null
        /// </summary>
        SoundConfig AttackSound { get; }

        /// <summary>
        /// Hit sound override, can be null
        /// </summary>
        SoundConfig HitSound { get; }

        /// <summary>
        /// Hit force, used to move ragdoll after killing target
        /// </summary>
        float HitForce { get; }

        #endregion
    }
}