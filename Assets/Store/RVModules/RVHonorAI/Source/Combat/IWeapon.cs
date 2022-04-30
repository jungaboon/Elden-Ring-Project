// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using System.Collections.Generic;
using RVModules.RVCommonGameLibrary.Audio;
using UnityEngine;

namespace RVHonorAI.Combat
{
    /// <summary>
    /// Definition of weapon
    /// All 'baseline' stats are weapon's  and can be overriden by attacks
    /// </summary>
    public interface IWeapon
    {
        #region Properties

//        /// <summary>
//        /// Attacks available for this weapon
//        /// </summary>
//        List<IAttack> Attacks { get; }

        /// <summary>
        /// Transform of this weapon
        /// </summary>
        Transform Transform { get; }

        /// <summary>
        /// Damage type
        /// </summary>
        DamageType DamageType { get; }

        /// <summary>
        /// Baseline attack damage
        /// </summary>
        float Damage { get; }

        /// <summary>
        /// Baseline attack range
        /// </summary>
        float Range { get; }

        /// <summary>
        /// Engange range of attack with the highest range
        /// </summary>
        float MaxEngageRange { get; }

        /// <summary>
        /// Baseline attack duration
        /// </summary>
        float AttackDuration { get; }

        /// <summary>
        /// Baseline attack angle in degrees
        /// </summary>
        float AttackAngle { get; }

        /// <summary>
        /// 
        /// </summary>
        AttackType AttackType { get; }

        /// <summary>
        /// 
        /// </summary>
        SoundConfig AttackSound { get; }

        /// <summary>
        /// 
        /// </summary>
        SoundConfig HitSound { get; }

        #endregion
    }
}