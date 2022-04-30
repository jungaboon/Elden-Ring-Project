// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVHonorAI.Combat;
using UnityEngine;

namespace RVHonorAI.Systems
{
    public class AttackSelectionSystem : ScriptableObject
    {
        #region Public methods

        /// <summary>
        /// Attack with the biggest returned score will be selected
        /// Default implementation returns score based on attack preference
        /// </summary>
        public virtual float ScoreAttack(ICharacter _attacker, IWeapon _currentWeapon, IAttack _atk, ITarget target) =>
            Random.Range(0, _atk.Preference);

        #endregion
    }
}