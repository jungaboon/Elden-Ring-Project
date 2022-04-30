// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using UnityEngine;

namespace RVHonorAI
{
    [CreateAssetMenu(fileName = "Ai group", menuName = "Honor AI/Ai group")]
    public class AiGroup : ScriptableObject
    {
        #region Fields

        public AiGroup[] allies;
        public AiGroup[] enemies;

        public bool enemyToAll;
        public bool allyToAll;

        #endregion
    }
}