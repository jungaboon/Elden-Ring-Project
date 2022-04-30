// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System;

namespace RVModules.RVSmartAI
{
    [Obsolete("This attribute is not needed anymore, expose field just like in normal MonoBehabiour - by making serialized field")]
    public class SmartAiExposeField : Attribute
    {
        #region Fields

        public string description;

        #endregion

        public SmartAiExposeField()
        {
        }

        [Obsolete("Use unity attributes like header or tooltip instead")]
        public SmartAiExposeField(string _description) => description = _description;
    }
}