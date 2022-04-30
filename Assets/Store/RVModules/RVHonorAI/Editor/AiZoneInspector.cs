// Created by Ronis Vision. All rights reserved
// 15.07.2020.

using RVModules.RVUtilities.Editor;
using UnityEditor;

namespace RVHonorAI.Editor
{
    [CustomEditor(typeof(AiZone))] public class AiZoneInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI() => EditorHelpers.WrapInBox(() => base.OnInspectorGUI());
    }
}