// Created by Ronis Vision. All rights reserved
// 31.12.2019.

namespace RVModules.RVSmartAI.Content.AI.ReflectionBased.Tasks
{
    public class SetBool : SetProperty
    {
        #region Fields


        public bool value;

        #endregion

        #region Properties

        protected override object Value => value;

        #endregion
    }
}