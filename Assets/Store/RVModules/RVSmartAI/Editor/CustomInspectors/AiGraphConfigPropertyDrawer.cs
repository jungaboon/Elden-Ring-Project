// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using RVModules.RVUtilities.Editor;
using UnityEditor;
using UnityEngine;

namespace RVModules.RVSmartAI.Editor.CustomInspectors
{
    [CustomPropertyDrawer(typeof(AiGraphConfig), true)]
    public class AiGraphConfigPropertyDrawer : PropertyDrawer
    {
        #region Public methods

        //public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) =>
            EditorHelpers.WrapInBox(() =>
            {
                EditorGUILayout.PropertyField(property.FindPropertyRelative("aiGraph"));
                EditorGUILayout.PropertyField(property.FindPropertyRelative("overrideGraphVariablesForNestedGraphs"));

                var customLbcProp = property.FindPropertyRelative("useCustomLoadBalancerConfig");

                if (customLbcProp.boolValue)
                {
                    EditorGUILayout.PropertyField(customLbcProp);
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("loadBalancerConfig"), true);
                }
                else
                {
                    var updateFreqProp = property.FindPropertyRelative("updateFrequency");
                    var updateFreq = updateFreqProp.intValue;
                    var maxAllowedFreq = updateFreq;

                    EditorGUILayout.PropertyField(customLbcProp);

                    var scalableAi = property.FindPropertyRelative("scalableLoadBalancing");
                    EditorGUILayout.PropertyField(scalableAi);
                    if (scalableAi.boolValue)
                    {
                        var maxFreqProp = property.FindPropertyRelative("maxAllowedUpdateFrequency");
                        EditorGUILayout.PropertyField(maxFreqProp);
                        maxAllowedFreq = maxFreqProp.intValue;
                    }
                    EditorGUILayout.IntSlider(updateFreqProp, 1, 8);

                    var expand = property.FindPropertyRelative("expandPerfInfo");
                    expand.boolValue = EditorGUILayout.Foldout(expand.boolValue, "Performance info");
                    if (expand.boolValue) GUIHelpers.AiLbPerfInfo(maxAllowedFreq, updateFreq, scalableAi.boolValue);

                }
            });

        #endregion
    }
}