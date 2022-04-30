// Created by Ronis Vision. All rights reserved
// 23.08.2019.

using RVModules.RVSmartAI.Content.Scanners;
using UnityEngine;

namespace RVModules.RVSmartAI.Content
{
    public class Obstacle : MonoBehaviour, IScannable
    {
        #region Properties

        public Object GetObject => this;

        #endregion
    }
}