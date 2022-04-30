// Created by Ronis Vision. All rights reserved
// 07.04.2021.

using System;
using UnityEngine;

namespace RVModules.RVUtilities
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ConditionalHideAttribute : PropertyAttribute
    {
        #region Fields

        public readonly string conditionalSourceField;
        public string conditionalSourceField2 = "";
        public bool hideInInspector = false;
        public bool inverse = false;

        #endregion

        // Use this for initialization
        public ConditionalHideAttribute(string _conditionalSourceField) => conditionalSourceField = _conditionalSourceField;
    }
}