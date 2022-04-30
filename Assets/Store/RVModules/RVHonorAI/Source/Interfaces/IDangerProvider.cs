// Created by Ronis Vision. All rights reserved
// 02.04.2021.

namespace RVHonorAI
{
    public interface IDangerProvider
    {
        #region Properties

        /// <summary>
        /// Value for determining generally how strong and dangerous this entity is
        /// It should take into account thing like hp, damage per second etc
        /// </summary>
        float Danger { get; }

        #endregion
    }
}