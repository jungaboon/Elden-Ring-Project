// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVHonorAI.Combat;
using UnityEngine;

namespace RVHonorAI.Systems
{
    /// <summary>
    /// Independent modules. Allows for better flexibility than overriding methods in Character/CharacterAi classes.
    /// todo extract distance attack system
    /// </summary>
    public class AiSystems : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private DamageSystem damageSystem;

        [SerializeField]
        private RelationshipSystem relationshipSystem;

        [SerializeField]
        private AttackSelectionSystem attackSelectionSystem;

        [SerializeField]
        private MeleeAttackSystem meleeAttackSystem;

        #endregion

        #region Properties

        public RelationshipSystem RelationshipSystem => relationshipSystem;

        public AttackSelectionSystem AttackSelectionSystem => attackSelectionSystem;

        public MeleeAttackSystem AttackSystem => meleeAttackSystem;

        public DamageSystem DamageSystem => damageSystem;

        #endregion
    }
}