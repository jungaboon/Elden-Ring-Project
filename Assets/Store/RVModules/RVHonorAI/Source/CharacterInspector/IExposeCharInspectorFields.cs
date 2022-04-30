// Created by Ronis Vision. All rights reserved
// 23.04.2021.

using RVModules.RVSmartAI;

namespace RVHonorAI.CharacterInspector
{
    /// <summary>
    /// Implement it to inform that you want some or all serialized fields of this component to be exposed in Character inspector 
    /// </summary>
    public interface IExposeCharInspectorFields : IComponent
    {
        #region Properties

        /// <summary>
        /// Return true if you want all serialized fields to be automatically drawed in Character inspector under DefaultCharInspectorTab tab
        /// </summary>
        bool ExposeAllFieldsToCharInspector { get; }

        /// <summary>
        /// If ExposeAllFieldsToCharInspector is set to true, all fields without specified tab will be drawn under this tab
        /// </summary>
        CharacterInspectorTab DefaultCharInspectorTab { get; }

        #endregion
    }
}