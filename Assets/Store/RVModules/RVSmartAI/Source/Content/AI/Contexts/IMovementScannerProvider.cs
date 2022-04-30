// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using RVModules.RVSmartAI.Content.Scanners;

namespace RVModules.RVSmartAI.Content.AI.Contexts
{
    public interface IMovementScannerProvider
    {
        #region Properties

        IMovementScanner MovementScanner { get; }

        #endregion
    }
}