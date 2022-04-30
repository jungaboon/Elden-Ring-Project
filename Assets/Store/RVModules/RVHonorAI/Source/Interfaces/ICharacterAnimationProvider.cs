// Created by Ronis Vision. All rights reserved
// 02.04.2021.

namespace RVHonorAI
{
    public interface ICharacterAnimationProvider
    {
        #region Properties

        /// <summary>
        /// CharacterAnimation component reference 
        /// </summary>
        ICharacterAnimation CharacterAnimation { get; }

        #endregion
    }
}