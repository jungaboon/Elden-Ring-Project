using FIMSpace.AnimationTools;
using FIMSpace.FEditor;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FProceduralAnimation
{
    [UnityEditor.CustomEditor(typeof(RagdollAnimator))]
    public partial class RagdollAnimatorEditor : UnityEditor.Editor
    {
        public RagdollAnimator Get { get { if (_get == null) _get = (RagdollAnimator)target; return _get; } }
        private RagdollAnimator _get;

        private SerializedProperty sp_RagProcessor;
        private SerializedProperty sp_ObjectWithAnimator;

        private RagdollGenerator generator;
        public static Texture2D Tex_Rag { get { if (__texRag != null) return __texRag; __texRag = Resources.Load<Texture2D>("Ragdoll Animator/Ragdoll"); return __texRag; } }
        private static Texture2D __texRag = null;

        bool drawAdditionalSettings = true;
        bool drawCorrectionsSettings = false;
        bool triggerGenerateRagd = false;

        private void OnEnable()
        {
            sp_RagProcessor = serializedObject.FindProperty("Processor");
            sp_ObjectWithAnimator = serializedObject.FindProperty("ObjectWithAnimator");

            if (Application.isPlaying)
            {
                Get._EditorDrawSetup = false;
            }
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxBlankStyle);

            //if (Application.isPlaying)
            //{
            //    Animator a = Get.transform.GetComponentInChildren<Animator>();
            //    if (a)
            //    {
            //        if (a.enabled == false)
            //        {
            //            GUILayout.Space(4);
            //            EditorGUILayout.HelpBox("Unity Animator disabled - Ragdoll Animator will apply it's algorithms", MessageType.None);
            //            if (GUILayout.Button("Test Enable Animator")) { Get.User_SwitchAnimator(null, true); Get.User_SetAllKinematic(); }
            //            GUILayout.Space(4);
            //        }
            //        else
            //        {
            //            GUILayout.Space(4);
            //            EditorGUILayout.HelpBox("! Unity Animator enabled - Ragdoll Animator overrided - not doing anything", MessageType.None);
            //            if (GUILayout.Button("Test Disable Animator")) { Get.User_SwitchAnimator(); Get.User_SetAllKinematic(false); }
            //            GUILayout.Space(4);
            //        }
            //    }
            //}

            serializedObject.Update();

            Editor_DrawTweakFullGUI(sp_RagProcessor, Get.Parameters, ref Get._EditorDrawSetup);

            GUILayout.Space(2);
            //GUILayout.Space(6);

            if (Get._EditorDrawSetup)
            {
                FGUI_Inspector.DrawUILine(0.4f, 0.3f, 1, 8);

                EditorGUILayout.PropertyField(sp_ObjectWithAnimator);
                var sp = sp_ObjectWithAnimator.Copy(); sp.Next(false);
                EditorGUILayout.PropertyField(sp); //sp.Next(false);
                //EditorGUILayout.PropertyField(sp); sp.Next(false);
                //EditorGUILayout.PropertyField(sp); sp.Next(false);
            }

            GUILayout.Space(4);

            Undo.RecordObject(target, "RagdollAnimator");

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();


            if (triggerGenerateRagd)
            {
                if (Get.PreGenerateDummy)
                {
                    Get.Parameters.PreGenerateDummy(Get.ObjectWithAnimator, Get.RootBone);
                }
                else
                {
                    Get.Parameters.RemovePreGeneratedDummy();
                }

                EditorUtility.SetDirty(Get);
                triggerGenerateRagd = false;
            }

        }


        public void Editor_DrawTweakFullGUI(SerializedProperty sp_ragProcessor, RagdollProcessor proc, ref bool drawSetup)
        {
            Color bg = GUI.backgroundColor;

            GUILayout.Space(4);
            EditorGUILayout.BeginHorizontal();

            if (drawSetup) GUI.backgroundColor = new Color(1f, 1f, 0f);
            if (GUILayout.Button(new GUIContent(FGUI_Resources.Tex_GearSetup), FGUI_Resources.ButtonStyle, new GUILayoutOption[] { GUILayout.Width(28), GUILayout.Height(24) })) drawSetup = !drawSetup;
            GUI.backgroundColor = bg;

            EditorGUILayout.BeginVertical(FGUI_Resources.ViewBoxStyle);
            GUILayout.Space(2);
            EditorGUILayout.LabelField(drawSetup ? "  Setup Ragdoll Bones" : "Ragdoll Factors", FGUI_Resources.HeaderStyle);
            GUILayout.Space(1);

            EditorGUILayout.EndVertical();

            if (drawSetup)
                if (GUILayout.Button(new GUIContent(" Tutorial", FGUI_Resources.Tex_Tutorials), FGUI_Resources.ButtonStyle, GUILayout.Height(20), GUILayout.Width(90)))
                {
                    Application.OpenURL("https://youtu.be/dC5h-kVR650");
                }

            EditorGUILayout.EndHorizontal();

            if (drawSetup)
            {
                //GUILayout.Space(8);
                // Generating buttons etc.

                FGUI_Inspector.FoldHeaderStart(ref proc._EditorDrawBones, "  Bones Setup", FGUI_Resources.BGInBoxStyle, FGUI_Resources.Tex_Bone);

                if (proc._EditorDrawBones)
                {
                    GUILayout.Space(4);
                    SerializedProperty sp_BaseTransform = sp_ragProcessor.FindPropertyRelative("BaseTransform");

                    EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);
                    EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);
                    GUILayout.Space(8);
                    EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);

                    if (sp_BaseTransform.objectReferenceValue == null)
                        EditorGUILayout.PropertyField(sp_BaseTransform, new GUIContent("Chest (Optional)", sp_BaseTransform.tooltip));
                    else
                        EditorGUILayout.PropertyField(sp_BaseTransform);

                    sp_BaseTransform.Next(false);

                    EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);
                    GUILayout.Space(8);
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);
                    if (EditorGUI.EndChangeCheck()) { serializedObject.ApplyModifiedProperties(); if (Get.Parameters.LeftUpperArm != null) Get.Parameters.LeftForeArm = Get.Parameters.LeftUpperArm.GetChild(0); }
                    EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);
                    GUILayout.Space(5);
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);
                    if (EditorGUI.EndChangeCheck()) { serializedObject.ApplyModifiedProperties(); if (Get.Parameters.RightUpperArm != null) Get.Parameters.RightForeArm = Get.Parameters.RightUpperArm.GetChild(0); }
                    EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);
                    GUILayout.Space(8);
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);
                    if (EditorGUI.EndChangeCheck()) { serializedObject.ApplyModifiedProperties(); if (Get.Parameters.LeftUpperLeg != null) Get.Parameters.LeftLowerLeg = Get.Parameters.LeftUpperLeg.GetChild(0); }
                    EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);
                    GUILayout.Space(5);
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);
                    if (EditorGUI.EndChangeCheck()) { serializedObject.ApplyModifiedProperties(); if (Get.Parameters.RightUpperLeg != null) Get.Parameters.RightLowerLeg = Get.Parameters.RightUpperLeg.GetChild(0); }
                    EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);
                    GUILayout.Space(2);

                    if (Get.ObjectWithAnimator)
                    {
                        bool isHuman = false;
                        Animator anim = Get.ObjectWithAnimator.GetComponent<Animator>();
                        if (anim) isHuman = anim.isHuman;

                        if (!isHuman)
                        {
                            EditorGUILayout.HelpBox("Detected Generic or Legacy Rig, you want to try auto-detect limbs? (Verify them)", MessageType.None);

                            if (GUILayout.Button(new GUIContent("  Run Auto-Limb Detection Algorithm\n  <size=10>(Character must contain correct T-Pose)\n(And be Facing it's Z-Axis)</size>", FGUI_Resources.TexWaitIcon), FGUI_Resources.ButtonStyleR, GUILayout.Height(52)))
                            {
                                SkeletonRecognize.SkeletonInfo info = new SkeletonRecognize.SkeletonInfo(Get.ObjectWithAnimator);

                                #region Assigning found bones

                                int assigned = 0;
                                if (info.LeftArms > 0)
                                {
                                    if (info.ProbablyLeftArms[0].Count > 2)
                                    {
                                        assigned += 2;
                                        Get.Parameters.LeftUpperArm = info.ProbablyLeftArms[0][1];
                                        Get.Parameters.LeftForeArm = info.ProbablyLeftArms[0][2];
                                    }
                                }

                                if (info.RightArms > 0)
                                {
                                    if (info.ProbablyRightArms[0].Count > 2)
                                    {
                                        assigned += 2;
                                        Get.Parameters.RightUpperArm = info.ProbablyRightArms[0][1];
                                        Get.Parameters.RightForeArm = info.ProbablyRightArms[0][2];
                                    }
                                }


                                if (info.LeftLegs > 0)
                                {
                                    if (info.ProbablyLeftLegs[0].Count > 1)
                                    {
                                        assigned += 2;
                                        Get.Parameters.LeftUpperLeg = info.ProbablyLeftLegs[0][0];
                                        Get.Parameters.LeftLowerLeg = info.ProbablyLeftLegs[0][1];
                                    }
                                }

                                if (info.RightLegs > 0)
                                {
                                    if (info.ProbablyRightLegs[0].Count > 1)
                                    {
                                        assigned += 2;
                                        Get.Parameters.RightUpperLeg = info.ProbablyRightLegs[0][0];
                                        Get.Parameters.RightLowerLeg = info.ProbablyRightLegs[0][1];
                                    }
                                }

                                if (info.ProbablyHead)
                                {
                                    assigned += 1;
                                    Get.Parameters.Head = info.ProbablyHead;
                                }

                                if (info.ProbablyHips)
                                {
                                    assigned += 1;
                                    Get.Parameters.Pelvis = info.ProbablyHips;
                                }

                                if (info.SpineChainLength > 1)
                                {
                                    assigned += 2;
                                    Get.Parameters.SpineStart = info.ProbablySpineChain[0];

                                    int shortSp = info.ProbablySpineChainShort.Count;

                                    if (shortSp < 3)
                                        Get.Parameters.Chest = info.ProbablySpineChainShort[1];
                                    else
                                        if (shortSp > 2)
                                        Get.Parameters.Chest = info.ProbablySpineChainShort[shortSp - 1];

                                    if (Get.Parameters.Chest == Get.Parameters.Head) Get.Parameters.Chest = Get.Parameters.Chest.parent;
                                }

                                if (assigned < 2)
                                    EditorUtility.DisplayDialog("Auto Detection Report", "Couldn't detect bones on the current rig!", "Ok");
                                else
                                    EditorUtility.DisplayDialog("Auto Detection Report", "Found and Assigned " + assigned + " bones to help out faster setup. Please verify the new added bones", "Ok");

                                #endregion

                            }

                        }

                    }
                }

                GUILayout.EndVertical();

                GUILayout.Space(5);

                FGUI_Inspector.FoldHeaderStart(ref proc._EditorDrawGenerator, "  Ragdoll Generator", FGUI_Resources.BGInBoxStyle, FGUI_Resources.Tex_Collider);

                if (proc._EditorDrawGenerator)
                {
                    GUILayout.Space(3);
                    if (generator == null)
                    {
                        generator = new RagdollGenerator();
                        generator.BaseTransform = Get.ObjectWithAnimator != null ? Get.ObjectWithAnimator : Get.transform;
                        generator.SetAllBoneReferences(Get.Parameters.Pelvis, Get.Parameters.SpineStart, Get.Parameters.Chest, Get.Parameters.Head, Get.Parameters.LeftUpperArm, Get.Parameters.LeftForeArm, Get.Parameters.RightUpperArm, Get.Parameters.RightForeArm, Get.Parameters.LeftUpperLeg, Get.Parameters.LeftLowerLeg, Get.Parameters.RightUpperLeg, Get.Parameters.RightLowerLeg);
                    }

                    generator.Tab_RagdollGenerator(Get.Parameters, true);

                }
                else
                {
                    if (generator != null)
                        generator.ragdollTweak = RagdollGenerator.tweakRagd.None;
                }

                GUILayout.EndVertical();

                GUILayout.Space(7);

                FGUI_Inspector.FoldHeaderStart(ref drawAdditionalSettings, "  Additional Settings", FGUI_Resources.BGInBoxStyle, FGUI_Resources.Tex_GearSetup);

                if (drawAdditionalSettings)
                {
                    GUILayout.Space(3);
                    SerializedProperty sp_ext = sp_ragProcessor.FindPropertyRelative("ExtendedAnimatorSync");
                    EditorGUILayout.PropertyField(sp_ext);
                    sp_ext.NextVisible(false);
                    /*EditorGUILayout.PropertyField(sp_ext); */

                    EditorGUILayout.BeginHorizontal();
                    DisableOnPlay();
                    EditorGUILayout.PropertyField(sp_ext); sp_ext.NextVisible(false);
                    DisableOnPlay(false);

                    if (proc.HipsPin == false)
                        EditorGUILayout.PropertyField(sp_ext);
                    EditorGUILayout.EndHorizontal();
                    DisableOnPlay();

                    var sp = sp_ObjectWithAnimator.Copy(); sp.Next(false); sp.Next(false);
                    EditorGUILayout.BeginHorizontal();
                    sp.Next(false); EditorGUILayout.PropertyField(sp);
                    sp.Next(false);


                    if (Get.RootBone)
                        if (Get.PreGenerateDummy || (Get.Parameters.LeftUpperArm && Get.Parameters.LeftUpperArm.GetComponent<Rigidbody>()))
                        {
                            if (Application.isPlaying) GUI.enabled = false;

                            bool preV = sp.boolValue;
                            EditorGUILayout.PropertyField(sp);

                            if (preV != sp.boolValue)
                            {
                                triggerGenerateRagd = true;
                            }

                            #region Debugging Backup
                            //if (sp.boolValue)
                            //{
                            //    if (GUILayout.Button("Pre Generate Dummy"))
                            //    {
                            //        sp.boolValue = !sp.boolValue;
                            //        Get.Parameters.PreGenerateDummy(Get.ObjectWithAnimator, Get.RootBone);
                            //    }
                            //}
                            //else
                            //{
                            //    GUI.backgroundColor = new Color(0.75f, 0.75f, 0.75f, 1f);
                            //    if (GUILayout.Button("Undo Dummy"))
                            //    {
                            //        sp.boolValue = !sp.boolValue;
                            //        Get.Parameters.RemovePreGeneratedDummy();
                            //    }
                            //    GUI.backgroundColor = Color.white;
                            //}
                            #endregion

                            if (Application.isPlaying) GUI.enabled = true;
                        }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    sp.Next(false); EditorGUILayout.PropertyField(sp);
                    if (Get.TargetParentForRagdollDummy == null)
                    {
                        if (GUILayout.Button("Self", GUILayout.Width(40)))
                        {
                            Get.TargetParentForRagdollDummy = Get.transform;
                            EditorUtility.SetDirty(Get);
                        }
                    }
                    EditorGUILayout.EndHorizontal(); DisableOnPlay(false);
                    GUILayout.Space(3);

                    GUILayout.Space(6);
                    FGUI_Inspector.FoldHeaderStart(ref drawCorrectionsSettings, "  Corrections", FGUI_Resources.BGInBoxStyle, FGUI_Resources.Tex_Tweaks);

                    if (drawCorrectionsSettings)
                    {
                        GUILayout.Space(3);
                        EditorGUILayout.BeginHorizontal();
                        sp_ext.NextVisible(false); DisableOnPlay(); EditorGUILayout.PropertyField(sp_ext);
                        sp_ext.NextVisible(false); EditorGUILayout.PropertyField(sp_ext);
                        EditorGUILayout.EndHorizontal(); DisableOnPlay(false);

                        EditorGUILayout.BeginHorizontal();
                        sp_ext.NextVisible(false); EditorGUILayout.PropertyField(sp_ext);
                        sp_ext.NextVisible(false); EditorGUILayout.PropertyField(sp_ext);
                        EditorGUILayout.EndHorizontal();

                        DisableOnPlay();
                        var sp2 = sp_ObjectWithAnimator.Copy(); sp2.Next(false);
                        EditorGUILayout.PropertyField(sp2);
                        sp2.Next(false); EditorGUILayout.PropertyField(sp2);

                        GUILayout.Space(3);
                        sp2 = sp_ragProcessor.FindPropertyRelative("Persistent");
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(sp2); DisableOnPlay(false); sp2.Next(false);
                        EditorGUILayout.PropertyField(sp2);
                        EditorGUILayout.EndHorizontal();
                    }

                    GUILayout.EndVertical();
                    GUILayout.Space(6);
                }

                GUILayout.EndVertical();
            }
            else
            {
                if (generator != null)
                    generator.ragdollTweak = RagdollGenerator.tweakRagd.None;

                GUILayout.Space(2);

                EditorGUILayout.BeginVertical(FGUI_Resources.ViewBoxStyle); // ----------
                GUILayout.Space(2);
                SerializedProperty sp_param = sp_ragProcessor.FindPropertyRelative("FreeFallRagdoll");

                RagdollProcessor.Editor_DrawTweakGUI(sp_param, Get.Parameters);

                EditorGUILayout.EndVertical();
            }


            if (Get.Parameters.Pelvis != null)
            {
                bool layerWarn = false;
                if (Get.gameObject.layer == Get.Parameters.Pelvis.gameObject.layer) layerWarn = true;
                if (Get.Parameters.SpineStart) if (Get.gameObject.layer == Get.Parameters.SpineStart.gameObject.layer) layerWarn = true;
                if (Get.Parameters.LeftUpperArm) if (Get.gameObject.layer == Get.Parameters.LeftUpperArm.gameObject.layer) layerWarn = true;

                if (layerWarn)
                {
                    GUILayout.Space(7);
                    EditorGUILayout.HelpBox("WARNING! It seams your main object have the same layer as bone transforms! You should create layer with ignored collision between character model and skeleton bones!", MessageType.Warning);
                    GUILayout.Space(7);
                }
            }
        }

        void DisableOnPlay(bool disable = true)
        {
            if (Application.isPlaying)
            {
                if (disable)
                    GUI.color = new Color(.9f, .9f, .9f, .5f);
                else
                    GUI.color = Color.white;
            }
        }

        private void OnSceneGUI()
        {
            if (generator == null) return;
            generator.OnSceneGUI();
        }

    }
}
