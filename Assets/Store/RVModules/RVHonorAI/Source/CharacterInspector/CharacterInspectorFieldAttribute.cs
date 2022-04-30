// Created by Ronis Vision. All rights reserved
// 23.04.2021.

using System;

namespace RVHonorAI.CharacterInspector
{
    public class CharacterInspectorFieldAttribute : Attribute
    {
        #region Fields

        public CharacterInspectorTab tab = CharacterInspectorTab.None;
        public bool drawWhenNotPlaying = true;
        public bool drawWhenPlaying = true;

        #endregion

        public CharacterInspectorFieldAttribute(CharacterInspectorTab _tab) => tab = _tab;

        public CharacterInspectorFieldAttribute()
        {
        }
    }
}