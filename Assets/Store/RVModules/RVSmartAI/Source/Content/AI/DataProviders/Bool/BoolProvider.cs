// Created by Ronis Vision. All rights reserved
// 13.10.2020.

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public abstract class BoolProvider : DataProvider<bool>
    {
        #region Public methods

        public static implicit operator bool(BoolProvider _boolProvider) => _boolProvider.GetData();

        #endregion
    }
}