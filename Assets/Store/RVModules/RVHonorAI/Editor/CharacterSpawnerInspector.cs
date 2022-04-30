// Created by Ronis Vision. All rights reserved
// 16.01.2020.

using System.Collections.Generic;
using RVModules.RVUtilities.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace RVHonorAI.Editor
{
    [CustomEditor(typeof(CharacterSpawner))] [CanEditMultipleObjects]
    public class CharacterSpawnerInspector : UnityEditor.Editor
    {
        #region Fields

        //private int tab = 0;

//        private Dictionary<string, SerializedProperty> serializedProperties = new Dictionary<string, SerializedProperty>();
        private ReorderableList rl;

        private CharacterSpawner characterSpawner;

        #endregion

        #region Public methods

        public override void OnInspectorGUI()
        {
            EditorHelpers.WrapInBox(() =>
            {
                base.OnInspectorGUI();

                serializedObject.Update();

                GeneralTab();
            });
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Not public methods

        private void OnEnable()
        {
//            EditorHelpers.AddProperty(serializedProperties, "walkSound", serializedObject);
            characterSpawner = (CharacterSpawner) target;

            rl = EditorHelpers.InitReorderableList(serializedObject.FindProperty("waypoints"), serializedObject);
        }

        private void GeneralTab()
        {
            EditorGUILayout.LabelField("Waypoints");
            rl.DoLayoutList();
            if (GUILayout.Button("Spawn")) characterSpawner.Spawn();
        }

        private void OnSceneGUI()
        {
            HonorAiEditorHelpers.WaypointsHandles(characterSpawner.waypoints, characterSpawner.transform.position, characterSpawner);
            Handles.color = Color.red;

            characterSpawner = (CharacterSpawner) target;
            if (characterSpawner.useBoxSizeSpawn)
                Handles.DrawWireCube(characterSpawner.transform.position, new Vector3(characterSpawner.size.x, 10, characterSpawner.size.y));
            else
                Handles.DrawWireDisc(characterSpawner.transform.position, Vector3.up, characterSpawner.radius);
        }

        #endregion
    }
}