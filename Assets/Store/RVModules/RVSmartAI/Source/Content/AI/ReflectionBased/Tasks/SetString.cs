// Created by Ronis Vision. All rights reserved
// 31.12.2019.

namespace RVModules.RVSmartAI.Content.AI.ReflectionBased.Tasks
{
    public class SetString : SetProperty
    {
        #region Fields


        public string value;

        #endregion

        #region Properties

        protected override object Value => value;

        #endregion
    }
}