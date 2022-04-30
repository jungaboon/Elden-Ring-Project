// Created by Ronis Vision. All rights reserved
// 26.07.2021.

using System;
using System.IO;
using RVModules.RVCommonGameLibrary.Tools;
using RVModules.RVSmartAI;
using RVModules.RVSmartAI.Content;
using UnityEditor;
using UnityEngine;

namespace RVHonorAI.Editor
{
    /// <summary>
    /// todo backport to componentsTools
    /// </summary>
    public class UpgradeTool : EditorWindow
    {
        [MenuItem("RonisVision/HonorAI/1.1 prefab upgrade tool")]
        private static void Init()
        {
            var window = (UpgradeTool) GetWindow(typeof(UpgradeTool));
            window.Show();
            window.titleContent.text = "HonorAI 1.1 upgrade tool";
        }

        private void OnGUI()
        {
            if (HonorVersion.Version == 0)
            {
                BeforeUpdate();
                AfterUpdate();
                return;
            }

            if (HonorVersion.Version < 1.1f)
            {
                BeforeUpdate();
            }
            else
            {
                AfterUpdate();
            }
        }

        private static void AfterUpdate()
        {
            EditorGUILayout.HelpBox("Select all your AI prefabs and press update prefabs. " +
                                    "After that and making sure all data is restored to your prefabs components you can remove all <prefabName> data.txt files " +
                                    "next to your prefabs", MessageType.Info);

            var selectedGos = Selection.GetFiltered<GameObject>(SelectionMode.Assets);

            if (selectedGos.Length > 0)
            {
                if (GUILayout.Button("Update prefabs"))
                {
                    foreach (var selectedGameObject in selectedGos)
                    {
                        var prefabPath = AssetDatabase.GetAssetPath(selectedGameObject);
                        if (string.IsNullOrEmpty(prefabPath)) continue;
                        var prefabDir = new FileInfo(prefabPath).DirectoryName;
                        if (prefabDir == null) continue;
                        var fileName = Path.Combine(prefabDir, $"{selectedGameObject.name} data.txt");
                        if (!File.Exists(fileName))
                        {
                            Debug.Log($"Data file {fileName} doesn't exist!");
                            continue;
                        }

                        Debug.Log($"Updating prefab {selectedGameObject.name}", selectedGameObject);

                        try
                        {
                            AddCharacterComponents(selectedGameObject);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Failed to add char components on {selectedGameObject}, error: {e}", selectedGameObject);
                            continue;
                        }

                        try
                        {
                            ComponentsSerialization.ReadComponentsData(
                                selectedGameObject.GetComponentsInChildren<Component>(),
                                fileName,
                                null, true);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Setting data for {selectedGameObject.name} failed, error: {e}", selectedGameObject);
                            continue;
                        }

                        Debug.Log($"Setting data for {selectedGameObject.name} successful, file path: {fileName}", selectedGameObject);
                    }

                    Debug.Log("Setting data for prefabs complete!");
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Select at least one prefab", MessageType.Info);
            }
        }

        private static void BeforeUpdate()
        {
            EditorGUILayout.HelpBox("Select all your AI prefabs and press serialize data. After that you can download and import " +
                                    "1.1 or newer version of HonorAI. After installing it content of this window will change " +
                                    "to show you next steps.", MessageType.Info);

            var selectedGos = Selection.GetFiltered<GameObject>(SelectionMode.Assets);

            if (selectedGos.Length > 0)
            {
                if (GUILayout.Button("Serialize prefabs data"))
                {
                    foreach (var selectedGameObject in selectedGos)
                    {
                        var prefabPath = AssetDatabase.GetAssetPath(selectedGameObject);
                        if (string.IsNullOrEmpty(prefabPath)) continue;
                        var prefabDir = new FileInfo(prefabPath).DirectoryName;
                        if (prefabDir == null) continue;
                        var fileName = Path.Combine(prefabDir, $"{selectedGameObject.name} data.txt");
                        if (File.Exists(fileName))
                        {
                            Debug.Log($"Asset {fileName} already exist, skipping");
                            continue;
                        }

                        // Debug.Log(fileName);
                        Debug.Log($"Serializing data for prefab {selectedGameObject.name}", selectedGameObject);
                        // continue;
                        try
                        {
                            ComponentsSerialization.SerializeComponents(
                                selectedGameObject.GetComponentsInChildren<Component>(),
                                fileName,
                                null, true);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Serialization for {selectedGameObject.name} failed, error: {e}", selectedGameObject);
                            continue;
                        }

                        Debug.Log($"Serialization for {selectedGameObject.name} successful, file path: {fileName}", selectedGameObject);
                    }

                    Debug.Log("Prefabs data serialized! You can now safely update HonorAI!");
                }
            }
        }

        public static void AddCharacterComponents(GameObject _gameObject)
        {
            if (_gameObject == null) return;
            ICharacter character = _gameObject.GetComponent<ICharacter>();
            if (character == null) return;

            Debug.Log($"Adding components to {_gameObject.name}", _gameObject);

            // var charGo = character.GameObject();
            // if (charGo.GetComponent<ICharacterDamage>() == null)
            // {
            //     Undo.AddComponent<CharacterDamage>(charGo);
            //     Debug.Log($"Added CharacterDamage to {_gameObject.name}", _gameObject);
            // }
            //
            // if (charGo.GetComponent<ICharacterAi>() == null)
            // {
            //     Undo.AddComponent<CharacterAi>(charGo);
            //     Debug.Log($"Added CharacterAi to {_gameObject.name}", _gameObject);
            // }
            //
            // if (charGo.GetComponent<IMovement>() == null)
            // {
            //     Undo.AddComponent<CharacterMovement>(charGo);
            //     Debug.Log($"Added CharacterMovement to {_gameObject.name}", _gameObject);
            // }
            //
            // if (charGo.GetComponent<ICharacterRelationship>() == null)
            // {
            //     Undo.AddComponent<CharacterRelationship>(charGo);
            //     Debug.Log($"Added CharacterRelationship to {_gameObject.name}", _gameObject);
            // }
            //
            // if (charGo.GetComponent<ICharacterAnimation>() == null)
            // {
            //     Undo.AddComponent<CharacterAnimation>(charGo);
            //     Debug.Log($"Added CharacterAnimation to {_gameObject.name}", _gameObject);
            // }
            //
            // if (charGo.GetComponent<ICharacterAudio>() == null)
            // {
            //     Undo.AddComponent<CharacterAudio>(charGo);
            //     Debug.Log($"Added CharacterAudio to {_gameObject.name}", _gameObject);
            // }
            //
            // if (charGo.GetComponent<CharacterRagdoll>() == null)
            // {
            //     Undo.AddComponent<CharacterRagdoll>(charGo);
            //     Debug.Log($"Added CharacterRagdoll to {_gameObject.name}", _gameObject);
            // }
            //
            // if (charGo.GetComponent<CharacterInfoBarHandler>() == null)
            // {
            //     Undo.AddComponent<CharacterInfoBarHandler>(charGo);
            //     Debug.Log($"Added CharacterInfoBarHandler to {_gameObject.name}", _gameObject);
            // }
        }
    }
}