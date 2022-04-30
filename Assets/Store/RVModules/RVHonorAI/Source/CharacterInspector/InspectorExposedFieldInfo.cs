// Created by Ronis Vision. All rights reserved
// 22.04.2021.

namespace RVHonorAI.CharacterInspector
{
    /// <summary>
    /// Allows to expose additional serialized fields under relevant tabs in Character inspector
    /// </summary>
    public class InspectorExposedFieldInfo
    {
        #region Fields

        private CharacterInspectorTab group;
        private string name;

        #endregion

        #region Properties

        public CharacterInspectorTab Group => group;

        public string Name => name;

        #endregion

        public InspectorExposedFieldInfo(CharacterInspectorTab _group, string _name)
        {
            group = _group;
            name = _name;
        }
    }
}