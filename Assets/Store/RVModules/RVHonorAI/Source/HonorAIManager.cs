// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVUtilities;
using UnityEngine;

namespace RVHonorAI
{
    public class HonorAIManager : MonoSingleton<HonorAIManager>
    {
        #region Fields

        [SerializeField]
        internal int totalCharactersCount;

        [SerializeField]
        internal int activeCharactersCount;

        #endregion
    }
}