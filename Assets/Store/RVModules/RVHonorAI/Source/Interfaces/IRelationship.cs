// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI;

namespace RVHonorAI
{
    /// <summary>
    /// todo desc
    /// </summary>
    public interface IRelationship : IComponent
    {
        #region Properties

        /// <summary>
        /// Our ai group
        /// </summary>
        AiGroup AiGroup { get; set; }

        /// <summary>
        /// If other character is not ally to this, should it be treated as enemy
        /// </summary>
        bool TreatNeutralCharactersAsEnemies { get; }

        #endregion

        #region Public methods

        /// <summary>
        /// Check's relationship to other
        /// </summary>
        bool IsEnemy(IRelationship _other, bool _contraCheck = false);

        /// <summary>
        /// Check's relationship to other
        /// </summary>
        bool IsAlly(IRelationship _other);

        #endregion
    }
}