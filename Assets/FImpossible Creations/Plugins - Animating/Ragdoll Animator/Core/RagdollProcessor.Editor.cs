#if UNITY_EDITOR
using FIMSpace.FEditor;
using System;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FProceduralAnimation
{
    public partial class RagdollProcessor
    {
        [HideInInspector] public bool _EditorDrawBones = true;
        [HideInInspector] public bool _EditorDrawGenerator = false;
        [HideInInspector] public bool _EditorDrawMore = false;

        static bool _displayHipsPinSettings = false;

        public static void Editor_DrawTweakGUI(SerializedProperty sp_param, RagdollProcessor proc)
        {
            EditorGUILayout.PropertyField(sp_param);
            bool freeFall = sp_param.boolValue;
            sp_param.Next(false);

            float amount = sp_param.floatValue;

            Color preC = GUI.color;
            if (freeFall && amount < 0.5f) GUI.color = Color.yellow;
            EditorGUILayout.PropertyField(sp_param); sp_param.Next(false);
            if (freeFall && amount < 0.5f) GUI.color = preC;

            EditorGUILayout.PropertyField(sp_param); sp_param.Next(false);
            EditorGUILayout.PropertyField(sp_param); sp_param.Next(false);
            EditorGUILayout.PropertyField(sp_param); sp_param.Next(false);

            // constant blends
            EditorGUILayout.PropertyField(sp_param); sp_param.Next(false);
            EditorGUILayout.PropertyField(sp_param); sp_param.Next(false);
            EditorGUILayout.PropertyField(sp_param); sp_param.Next(false);

            // spring damping
            EditorGUILayout.PropertyField(sp_param); sp_param.Next(false);
            EditorGUILayout.PropertyField(sp_param); sp_param.Next(false);

            if (proc.HipsPin)
            {
                FGUI_Inspector.FoldHeaderStart(ref _displayHipsPinSettings, "Hips Pin Adjustements", EditorStyles.helpBox);
                if (_displayHipsPinSettings)
                {
                    GUILayout.Space(3);
                    EditorGUILayout.PropertyField(sp_param); sp_param.Next(false);
                    EditorGUILayout.PropertyField(sp_param); sp_param.Next(false);
                    EditorGUILayout.PropertyField(sp_param); sp_param.Next(false);
                    GUILayout.Space(3);
                }
                else
                {
                    sp_param.Next(false); sp_param.Next(false); sp_param.Next(false);
                }
                GUILayout.EndVertical();
            }
            else
            {
                sp_param.Next(false); sp_param.Next(false); sp_param.Next(false);
            }

            // more custom
            EditorGUILayout.PropertyField(sp_param); sp_param.Next(false);
            EditorGUILayout.PropertyField(sp_param); sp_param.Next(false);
            EditorGUILayout.PropertyField(sp_param);

            if (proc.lminArmsAnim > 0f || proc.minRArmsAnim > 0f || proc.minSpineAnim > 0f || proc.minHeadAnim > 0f || proc.minChestAnim > 0f)
            {
                bool preE = GUI.enabled; GUI.enabled = false;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayoutOption wdth = GUILayout.Width(90);

                if (proc.lminArmsAnim > 0.0001f)
                {
                    EditorGUILayout.Slider("Left Arm Blend In", proc.lminArmsAnim, 0f, 1f);

                    if (proc.posingLeftUpperArm.collisions.EnteredCollisions.Count > 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.ObjectField(proc.posingLeftUpperArm.transform, typeof(Transform), true);
                        EditorGUILayout.LabelField("Collides With", wdth);
                        EditorGUILayout.ObjectField(proc.posingLeftUpperArm.collisions.EnteredCollisions[0], typeof(Transform), true);
                        EditorGUILayout.EndHorizontal();
                    }

                    if (proc.posingLeftForeArm.collisions.EnteredCollisions.Count > 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.ObjectField(proc.posingLeftForeArm.transform, typeof(Transform), true);
                        EditorGUILayout.LabelField("Collides With", wdth);
                        EditorGUILayout.ObjectField(proc.posingLeftForeArm.collisions.EnteredCollisions[0], typeof(Transform), true);
                        EditorGUILayout.EndHorizontal();
                    }

                }

                if (proc.minRArmsAnim > 0.0001f)
                {
                    EditorGUILayout.Slider("Right Arm Blend In", proc.minRArmsAnim, 0f, 1f);

                    if (proc.posingRightUpperArm.collisions.EnteredCollisions.Count > 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.ObjectField(proc.posingRightUpperArm.transform, typeof(Transform), true);
                        EditorGUILayout.LabelField("Collides With", wdth);
                        EditorGUILayout.ObjectField(proc.posingRightUpperArm.collisions.EnteredCollisions[0], typeof(Transform), true);
                        EditorGUILayout.EndHorizontal();
                    }

                    if (proc.posingRightForeArm.collisions.EnteredCollisions.Count > 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.ObjectField(proc.posingRightForeArm.transform, typeof(Transform), true);
                        EditorGUILayout.LabelField("Collides With", wdth);
                        EditorGUILayout.ObjectField(proc.posingRightForeArm.collisions.EnteredCollisions[0], typeof(Transform), true);
                        EditorGUILayout.EndHorizontal();
                    }
                }

                if (proc.minHeadAnim > 0.0001f)
                {
                    EditorGUILayout.Slider("Head Blend In", proc.minHeadAnim, 0f, 1f);

                    if (proc.posingHead.collisions.EnteredCollisions.Count > 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.ObjectField(proc.posingHead.transform, typeof(Transform), true);
                        EditorGUILayout.LabelField("Collides With", wdth);
                        EditorGUILayout.ObjectField(proc.posingHead.collisions.EnteredCollisions[0], typeof(Transform), true);
                        EditorGUILayout.EndHorizontal();
                    }
                }

                if (proc.minChestAnim > 0.0001f)
                {
                    EditorGUILayout.Slider("Chest Blend In", proc.minChestAnim, 0f, 1f);

                    if (proc.posingChest != null)
                        if (proc.posingChest.transform != null)
                            if (proc.posingChest.collisions.EnteredCollisions.Count > 0)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.ObjectField(proc.posingChest.transform, typeof(Transform), true);
                                EditorGUILayout.LabelField("Collides With", wdth);
                                EditorGUILayout.ObjectField(proc.posingChest.collisions.EnteredCollisions[0], typeof(Transform), true);
                                EditorGUILayout.EndHorizontal();
                            }

                    if (proc.posingSpineStart != null)
                        if (proc.posingSpineStart.transform != null)
                            if (proc.posingSpineStart.collisions.EnteredCollisions.Count > 0)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.ObjectField(proc.posingSpineStart.transform, typeof(Transform), true);
                                EditorGUILayout.LabelField("Collides With", wdth);
                                EditorGUILayout.ObjectField(proc.posingSpineStart.collisions.EnteredCollisions[0], typeof(Transform), true);
                                EditorGUILayout.EndHorizontal();
                            }
                }

                if (proc.minSpineAnim > 0.0001f)
                {
                    EditorGUILayout.Slider("Spine Blend In", proc.minSpineAnim, 0f, 1f);
                }

                EditorGUILayout.EndVertical();
                GUI.enabled = preE;
            }


            FGUI_Inspector.FoldHeaderStart(ref proc._EditorDrawMore, "More Individual Limbs Settings", FGUI_Resources.BGInBoxStyle);
            if (proc._EditorDrawMore)
            {
                sp_param.Next(false); EditorGUILayout.PropertyField(sp_param);
                sp_param.Next(false); EditorGUILayout.PropertyField(sp_param);
                FGUI_Inspector.DrawUILine(0.3f, 0.3f, 1, 4);

                sp_param.Next(false); EditorGUILayout.PropertyField(sp_param);
                sp_param.Next(false); EditorGUILayout.PropertyField(sp_param);
                FGUI_Inspector.DrawUILine(0.3f, 0.3f, 1, 4);

                sp_param.Next(false); EditorGUILayout.PropertyField(sp_param);
                sp_param.Next(false); EditorGUILayout.PropertyField(sp_param);
            }

            GUILayout.EndVertical();

            //Editor_MainSettings(sp_ragProcessor.FindPropertyRelative("SideSwayPower"));
            //GUILayout.Space(2);
            //Editor_RootSwaySettings(sp_ragProcessor.FindPropertyRelative("FootsOrigin"));
            //GUILayout.Space(2);
            //Editor_SpineLeanSettings(sp_ragProcessor.FindPropertyRelative("SpineBlend"));
        }


        internal void DrawGizmos()
        {
            if (Pelvis == null) return;
            Gizmos.DrawLine(Pelvis.position, Pelvis.TransformPoint(PelvisToBase));

            if (RagdollLimbs != null)
                if (Application.isPlaying)
                {
                    Handles.color = new Color(0.4f, 1f, 0.4f, 0.8f);

                    foreach (var item in RagdollLimbs)
                    {
                        if (item == null) continue;
                        if (item.transform.parent == null) continue;
                        if (item.transform == posingPelvis.transform) continue;

                        if (item.transform == posingLeftLowerLeg.transform || item.transform == posingRightLowerLeg.transform)
                        {
                            if (item.transform.childCount > 0) FGUI_Handles.DrawBoneHandle(item.transform.position, item.transform.GetChild(0).position, 0.6f);
                        }
                        else if (item.transform == posingLeftForeArm.transform)
                        {
                            FGUI_Handles.DrawBoneHandle(item.transform.position, item.transform.TransformPoint(LForearmToHand), 0.6f);
                        }
                        else if (item.transform == posingRightForeArm.transform)
                        {
                            FGUI_Handles.DrawBoneHandle(item.transform.position, item.transform.TransformPoint(RForearmToHand), 0.6f);
                        }
                        else if (item.transform == posingHead.transform)
                        {
                            FGUI_Handles.DrawBoneHandle(item.transform.position, item.transform.TransformPoint(HeadToTip), 0.6f);
                        }

                        Joint j = item.GetComponent<Joint>();
                        if (j == null) continue;
                        if (j.connectedBody == null) continue;

                        FGUI_Handles.DrawBoneHandle(j.connectedBody.transform.position, item.transform.position, 0.6f);
                    }

                    Handles.color = Color.white;
                }
        }

    }
}

#endif