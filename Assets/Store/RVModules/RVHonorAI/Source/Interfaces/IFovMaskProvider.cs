// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using UnityEngine;

namespace RVHonorAI
{
    public interface IFovMaskProvider
    {
        #region Properties

        LayerMask FovMask { get; }

        #endregion
    }
}