using FIMSpace.FEditor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FProceduralAnimation
{


    [System.Serializable]
    public class RagdollGenerator
    {
        public Transform BaseTransform = null;
        bool generateRagdoll = false;
        bool characterJoints = false;
        float ragdollScale = .8f;
        float ragdollBounciness = 0.0f;
        float ragdollDamper = 0f;
        float ragdollDrag = .5f;
        float ragdollAngularDrag = 1.25f;
        float ragdollSprings = 0f;
        float ragdollMass = 1f;
        float projDist = .05f;
        float projAngle = 60f;
        RigidbodyInterpolation ragdInterpol = RigidbodyInterpolation.Interpolate;
        bool enableCollision = false;
        bool enablePreProcessing = false;
        public enum tweakRagd { None, Position, Scale };
        public tweakRagd ragdollTweak = tweakRagd.None;

        bool generateFists = false;
        bool generateFoots = false;
        bool useSymmetry = false;

        public void Tab_RagdollGenerator(RagdollProcessor proc, bool assignProcessorBones)
        {
            if (proc.IsPreGeneratedDummy)
            {
                EditorGUILayout.HelpBox("Pre generated ragdoll is not available for ragdoll generator / colliders adjustements, generate ragdoll and do adjustements before pre generating", MessageType.Info);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 116;

                EditorGUI.BeginChangeCheck();
                generateRagdoll = EditorGUILayout.Toggle("Generate Ragdoll", generateRagdoll, new GUILayoutOption[] { GUILayout.Width(160) });
                EditorGUIUtility.labelWidth = 0;

                if (!generateRagdoll) GUI.enabled = false;
                if (GUILayout.Button(characterJoints ? "Now Using Character Joints" : "Now Using Configurable Joints")) characterJoints = !characterJoints;
                EditorGUILayout.EndHorizontal();

                ragdollScale = EditorGUILayout.Slider("Scale Ragdoll", ragdollScale, 0.5f, 1.25f);
                ragdollDrag = EditorGUILayout.Slider("Drag", ragdollDrag, 0f, 1f);
                ragdollAngularDrag = EditorGUILayout.Slider("Angular Drag", ragdollAngularDrag, 0f, 3f);
                ragdollMass = EditorGUILayout.Slider("Mass Multiplier", ragdollMass, 0f, 4f);
                ragdInterpol = (RigidbodyInterpolation)EditorGUILayout.EnumPopup("Rigidbodies Interpolation", ragdInterpol);

                GUILayout.Space(5);
                generateFoots = EditorGUILayout.Toggle("Generate Foots", generateFoots);
                generateFists = EditorGUILayout.Toggle("Generate Fists", generateFists);

                GUILayout.Space(5);


                if (!generateRagdoll) GUI.enabled = true;

                GUILayout.Space(5);

                EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);


                EditorGUILayout.BeginHorizontal();
                ragdollTweak = (tweakRagd)EditorGUILayout.EnumPopup("Tweak Ragdoll Colliders", ragdollTweak);

                GUILayout.Space(4);
                EditorGUIUtility.labelWidth = 44;
                useSymmetry = EditorGUILayout.Toggle(new GUIContent("Symm", "(EXPERIMENTAL) Use Symmetry for tweaking colliders"), useSymmetry, GUILayout.Width(64));
                EditorGUIUtility.labelWidth = 0;
                EditorGUILayout.EndHorizontal();


                GUILayout.Space(5);

                EditorGUILayout.HelpBox("Remember about correct collision LAYERS on bone transforms and on the movement controller!", MessageType.None);

                if (EditorGUI.EndChangeCheck())
                    if (generateRagdoll)
                        if (proc.RagdollDummyBase == null)
                        {
                            if (assignProcessorBones)
                            {
                                SetAllBoneReferences(proc.Pelvis, proc.SpineStart, proc.Chest, proc.Head, proc.LeftUpperArm, proc.LeftForeArm, proc.RightUpperArm, proc.RightForeArm, proc.LeftUpperLeg, proc.LeftLowerLeg, proc.RightUpperLeg, proc.RightLowerLeg);
                            }

                            UpdateOrGenerateRagdoll(proc.BaseTransform, characterJoints, ragdollScale, ragdollBounciness, ragdollDamper, ragdollSprings, ragdollDrag, ragdollAngularDrag, ragdollMass, true, projAngle, projDist, enableCollision, enablePreProcessing, false, ragdInterpol);
                        }

                GUILayout.Space(3);
                if (GUILayout.Button("Remove Ragdoll Components on Bones")) { RemoveRagdoll(); generateRagdoll = false; }

                EditorGUILayout.EndVertical();
            }

        }

        public void Ragdoll_IgnoreCollision(Transform a, Transform b)
        {
            CapsuleCollider ca, cb;
            ca = a.GetComponent<CapsuleCollider>();
            cb = b.GetComponent<CapsuleCollider>();

            if (ca != null && cb != null) Physics.IgnoreCollision(ca, cb);
        }

        public void Ragdoll_ComputeArmAxis(Transform baseTransform, Transform bone, Transform child, ref Vector3 f, ref Vector3 r, ref Vector3 u)
        {
            if (child == null)
                f = bone.transform.InverseTransformDirection(bone.position - bone.GetChild(0).position).normalized;
            else
                f = bone.transform.InverseTransformDirection(bone.position - child.position).normalized;

            r = -bone.transform.InverseTransformDirection(baseTransform.forward);
            u = Vector3.Cross(f, r);
        }

        public void Ragdoll_ComputeAxis(Transform baseTransform, Transform bone, ref Vector3 f, ref Vector3 r, ref Vector3 u)
        {
            r = bone.transform.InverseTransformDirection(baseTransform.right);
            f = bone.transform.InverseTransformDirection(baseTransform.forward);
            u = bone.transform.InverseTransformDirection(baseTransform.up);
        }



        public void UpdateOrGenerateRagdoll(Transform baseTransform, bool characterJoints = true, float scale = 1f, float bounciness = 0f, float damper = 0f, float spring = 0f, float drag = 0.5f, float aDrag = 1f, float massMul = 1f, bool projection = true, float projAngle = 90, float projDistance = 0.05f, bool enCollision = false, bool preProcessing = false, bool addToFoots = false, RigidbodyInterpolation interpolation = RigidbodyInterpolation.Interpolate)
        {
            if (LeftUpperArm == null || PelvisBone == null || SpineRoot == null)
            {
                UnityEngine.Debug.Log("[Ragdoll Generator] No bones to generate ragdoll!");
                return;
            }

            if (!generateFists)
            {
                if (LeftForearm.childCount > 0) Ragdoll_RemoveFrom(LeftForearm.GetLimbChild());
                if (RightForearm.childCount > 0) Ragdoll_RemoveFrom(RightForearm.GetLimbChild());
            }

            if (!generateFoots)
            {
                if (LeftLowerLeg.childCount > 0) Ragdoll_RemoveFrom(LeftLowerLeg.GetLimbChild());
                if (RightLowerLeg.childCount > 0) Ragdoll_RemoveFrom(RightLowerLeg.GetLimbChild());
            }

            Transform nck = Head.parent;
            Transform Neck = Head.parent;

            Vector3 r = new Vector3(), u = new Vector3(), f = new Vector3();

            Transform chst = Chest;
            if (chst == null) chst = Head.parent;

            // Pelvis to head
            Ragdoll_RefreshComponents(characterJoints, PelvisBone, chst.position - PelvisBone.position, false, bounciness, damper, drag, aDrag, projection, projAngle, projDistance, enCollision, preProcessing);
            Transform t = PelvisBone;
            PelvisBone.RagdollBody().mass = 16f * massMul;
            PelvisBone.RagdollCollider().height = t.InverseTransformVector(chst.position - t.position).magnitude * 0.4f * scale;
            PelvisBone.RagdollCollider().center = t.InverseTransformVector(chst.position - t.position) / 8f;
            PelvisBone.RagdollCollider().radius = PelvisBone.RagdollCollider().height * 1f * scale;


            Ragdoll_RefreshComponents(characterJoints, SpineRoot, nck.position - SpineRoot.position, true, bounciness, damper, drag, aDrag, projection, projAngle, projDistance, enCollision, preProcessing);
            t = SpineRoot;
            SpineRoot.RagdollBody().mass = 9f * massMul;
            SpineRoot.RagdollCollider().height = t.InverseTransformVector(nck.position - t.position).magnitude * 0.52f * scale;
            SpineRoot.RagdollCollider().center = t.InverseTransformVector(nck.position - t.position) / 3.175f;
            SpineRoot.RagdollCollider().radius = SpineRoot.RagdollCollider().height * 0.55f * scale;

            Ragdoll_JointbodyConnect(SpineRoot, PelvisBone.RagdollBody());
            Ragdoll_Joint(SpineRoot, -40f, 25f, 10f, 10f, spring, damper);
            Ragdoll_ComputeAxis(baseTransform, SpineRoot, ref f, ref r, ref u);
            Ragdoll_JointAxis(SpineRoot, r, null, r, f, u);

            if (Chest != null)
            {
                Ragdoll_RefreshComponents(characterJoints, Chest, nck.position - Chest.position, true, bounciness, damper, drag, aDrag, projection, projAngle, projDistance, enCollision, preProcessing);
                t = Chest;
                Chest.RagdollBody().mass = 5f * massMul;
                Chest.RagdollCollider().height = t.InverseTransformVector(nck.position - t.position).magnitude * 0.6f * scale;
                Chest.RagdollCollider().center = t.InverseTransformVector(nck.position - t.position) / 1.675f;
                Chest.RagdollCollider().radius = Chest.RagdollCollider().height * 1f * scale;

                Ragdoll_JointbodyConnect(Chest, SpineRoot.RagdollBody());
                Ragdoll_Joint(Chest, -40f, 25f, 10f, 10f, spring, damper);
                Ragdoll_ComputeAxis(baseTransform, Chest, ref f, ref r, ref u);
                Ragdoll_JointAxis(Chest, r, null, r, f, u);
            }
            else
            {
                chst = SpineRoot;
            }

            Transform ht = Head;
            Transform hr = Neck;

            if (Head.childCount > 0) { ht = Head.GetChild(0); hr = Head; }

            Vector3 hdDir;
            if (Neck == null) hdDir = (chst.position - ht.position) * 0.8f;
            else hdDir = ht.position - hr.position;

            Ragdoll_RefreshComponents(characterJoints, Head, hdDir, true, bounciness, damper, drag, aDrag, projection, projAngle, projDistance, enCollision, preProcessing);
            t = Head;

            Head.RagdollBody().mass = 4f * massMul;
            Head.RagdollCollider().height = t.InverseTransformVector(hdDir).magnitude * 1.5f * scale;
            Head.RagdollCollider().center = t.InverseTransformVector(hdDir) / 2.7f;
            Head.RagdollCollider().radius = Head.RagdollCollider().height * 1.65f * scale;

            Ragdoll_JointbodyConnect(Head, chst.RagdollBody());
            Ragdoll_Joint(Head, -50f, 50f, 35f, 35f, spring, damper);
            Ragdoll_ComputeAxis(baseTransform, Head, ref f, ref r, ref u);
            Ragdoll_JointAxis(Head, r, null, r, f, u);

            // Left Arm
            ht = LeftForearm; hr = LeftUpperArm;
            Ragdoll_RefreshComponents(characterJoints, LeftUpperArm, ht.position - hr.position, true, bounciness, damper, drag, aDrag, projection, projAngle, projDistance, enCollision, preProcessing);
            t = LeftUpperArm;

            float upperArmShldDiv = 2.85f;

            LeftUpperArm.RagdollBody().mass = 3f * massMul;
            LeftUpperArm.RagdollCollider().height = t.InverseTransformVector(ht.position - hr.position).magnitude * 1.5f * scale;
            LeftUpperArm.RagdollCollider().center = t.InverseTransformVector(ht.position - hr.position) / upperArmShldDiv;
            LeftUpperArm.RagdollCollider().radius = LeftUpperArm.RagdollCollider().height * 0.235f * scale;

            Ragdoll_JointbodyConnect(LeftUpperArm, chst.RagdollBody());
            Ragdoll_Joint(LeftUpperArm, -45f, 55f, 25f, 55f, spring, damper);
            Ragdoll_ComputeArmAxis(baseTransform, LeftUpperArm, LeftForearm, ref f, ref r, ref u);
            Ragdoll_JointAxis(LeftUpperArm, r, -f, r, f, u);

            float fistsMul1 = 1.8f;
            float fistsMul2 = 1.75f;
            if (generateFists) { fistsMul1 = 1.25f; fistsMul2 = 2f; }

            {
                Transform LeftHand = LeftForearm.GetLimbChild();

                ht = LeftHand; hr = LeftForearm;
                Ragdoll_RefreshComponents(characterJoints, LeftForearm, ht.position - hr.position, true, bounciness, damper, drag, aDrag, projection, projAngle, projDistance, enCollision, preProcessing);
                t = LeftForearm;
                LeftForearm.RagdollBody().mass = 2f * massMul;
                LeftForearm.RagdollCollider().height = t.InverseTransformVector(ht.position - hr.position).magnitude * fistsMul1 * scale;
                LeftForearm.RagdollCollider().center = t.InverseTransformVector(ht.position - hr.position) / fistsMul2;
                LeftForearm.RagdollCollider().radius = LeftForearm.RagdollCollider().height * 0.175f * scale;

                Ragdoll_JointbodyConnect(LeftForearm, LeftUpperArm.RagdollBody());
                Ragdoll_Joint(LeftForearm, -35f, 5f, 12f, 75f, spring, damper);
                Ragdoll_ComputeArmAxis(baseTransform, LeftForearm, LeftHand, ref f, ref r, ref u);
                Ragdoll_JointAxis(LeftForearm, u, -f, r, f, u);
            }

            // Right Arm
            ht = RightForearm; hr = RightUpperArm;
            Ragdoll_RefreshComponents(characterJoints, RightUpperArm, ht.position - hr.position, true, bounciness, damper, drag, aDrag, projection, projAngle, projDistance, enCollision, preProcessing);
            t = RightUpperArm;
            RightUpperArm.RagdollBody().mass = 3f * massMul;
            RightUpperArm.RagdollCollider().height = t.InverseTransformVector(ht.position - hr.position).magnitude * 1.5f * scale;
            RightUpperArm.RagdollCollider().center = t.InverseTransformVector(ht.position - hr.position) / upperArmShldDiv;
            RightUpperArm.RagdollCollider().radius = RightUpperArm.RagdollCollider().height * 0.235f * scale;

            Ragdoll_JointbodyConnect(RightUpperArm, chst.RagdollBody());
            Ragdoll_Joint(RightUpperArm, -45f, 55f, 25f, 55f, spring, damper);
            Ragdoll_ComputeArmAxis(baseTransform, RightUpperArm, RightForearm, ref f, ref r, ref u);
            Ragdoll_JointAxis(RightUpperArm, -r, -f, r, f, u);


            Transform RightHand = RightForearm.GetLimbChild();
            ht = RightHand; hr = RightForearm;
            Ragdoll_RefreshComponents(characterJoints, RightForearm, ht.position - hr.position, true, bounciness, damper, drag, aDrag, projection, projAngle, projDistance, enCollision, preProcessing);
            t = RightForearm;
            RightForearm.RagdollBody().mass = 2f * massMul;
            RightForearm.RagdollCollider().height = t.InverseTransformVector(ht.position - hr.position).magnitude * fistsMul1 * scale;
            RightForearm.RagdollCollider().center = t.InverseTransformVector(ht.position - hr.position) / fistsMul2;
            RightForearm.RagdollCollider().radius = RightForearm.RagdollCollider().height * 0.175f * scale;

            Ragdoll_JointbodyConnect(RightForearm, RightUpperArm.RagdollBody());
            Ragdoll_Joint(RightForearm, -35f, 5f, 12f, 75f, spring, damper);
            Ragdoll_ComputeArmAxis(baseTransform, RightForearm, RightHand, ref f, ref r, ref u);
            Ragdoll_JointAxis(RightForearm, u, -f, r, f, u);


            // Left Leg
            ht = LeftLowerLeg; hr = LeftUpperLeg;
            Ragdoll_RefreshComponents(characterJoints, LeftUpperLeg, ht.position - hr.position, true, bounciness, damper, drag, aDrag, projection, projAngle, projDistance, enCollision, preProcessing);
            t = LeftUpperLeg;
            LeftUpperLeg.RagdollBody().mass = 5f * massMul;
            LeftUpperLeg.RagdollCollider().height = t.InverseTransformVector(ht.position - hr.position).magnitude * 1.15f * scale;
            LeftUpperLeg.RagdollCollider().center = t.InverseTransformVector(ht.position - hr.position) / 2f;
            LeftUpperLeg.RagdollCollider().radius = LeftUpperLeg.RagdollCollider().height * 0.21f * scale;

            Ragdoll_JointbodyConnect(LeftUpperLeg, PelvisBone.RagdollBody());
            Ragdoll_Joint(LeftUpperLeg, -60f, 60f, 15f, 24f, spring, damper);
            Ragdoll_ComputeAxis(baseTransform, LeftUpperLeg, ref f, ref r, ref u);
            Ragdoll_JointAxis(LeftUpperLeg, r, null, r, f, u);


            float footsMul = 1.5f;
            float footsMul2 = 1.75f;
            if (generateFoots) { footsMul = 1.15f; footsMul2 = 2.1f; }

            Transform lFoot = LeftLowerLeg.GetLimbChild();
            ht = lFoot; hr = LeftLowerLeg;
            Ragdoll_RefreshComponents(characterJoints, LeftLowerLeg, ht.position - hr.position, true, bounciness, damper, drag, aDrag, projection, projAngle, projDistance, enCollision, preProcessing);
            t = LeftLowerLeg;
            LeftLowerLeg.RagdollBody().mass = 3f * massMul;
            LeftLowerLeg.RagdollCollider().height = t.InverseTransformVector(ht.position - hr.position).magnitude * footsMul * scale;
            LeftLowerLeg.RagdollCollider().center = t.InverseTransformVector(ht.position - hr.position) / footsMul2;
            LeftLowerLeg.RagdollCollider().radius = LeftLowerLeg.RagdollCollider().height * 0.145f * scale;

            Ragdoll_JointbodyConnect(LeftLowerLeg, LeftUpperLeg.RagdollBody());
            Ragdoll_Joint(LeftLowerLeg, -155f, 1f, 15f, 15f, spring, damper);
            Ragdoll_ComputeAxis(baseTransform, LeftLowerLeg, ref f, ref r, ref u);
            Ragdoll_JointAxis(LeftLowerLeg, r, null, r, f, u);


            // Right Leg
            ht = RightLowerLeg; hr = RightUpperLeg;
            Ragdoll_RefreshComponents(characterJoints, RightUpperLeg, ht.position - hr.position, true, bounciness, damper, drag, aDrag, projection, projAngle, projDistance, enCollision, preProcessing);
            t = RightUpperLeg;
            RightUpperLeg.RagdollBody().mass = 5f * massMul;
            RightUpperLeg.RagdollCollider().height = t.InverseTransformVector(ht.position - hr.position).magnitude * 1.15f * scale;
            RightUpperLeg.RagdollCollider().center = t.InverseTransformVector(ht.position - hr.position) / 2f;
            RightUpperLeg.RagdollCollider().radius = RightUpperLeg.RagdollCollider().height * 0.21f * scale;

            Ragdoll_JointbodyConnect(RightUpperLeg, PelvisBone.RagdollBody());
            Ragdoll_Joint(RightUpperLeg, -60f, 60f, 15f, 24f, spring, damper);
            Ragdoll_ComputeAxis(baseTransform, RightUpperLeg, ref f, ref r, ref u);
            Ragdoll_JointAxis(RightUpperLeg, r, null, r, f, u);


            Transform rFoot = RightLowerLeg.GetLimbChild();
            ht = rFoot; hr = RightLowerLeg;
            Ragdoll_RefreshComponents(characterJoints, RightLowerLeg, ht.position - hr.position, true, bounciness, damper, drag, aDrag, projection, projAngle, projDistance, enCollision, preProcessing);
            t = RightLowerLeg;
            RightLowerLeg.RagdollBody().mass = 3f * massMul;
            RightLowerLeg.RagdollCollider().height = t.InverseTransformVector(ht.position - hr.position).magnitude * footsMul * scale;
            RightLowerLeg.RagdollCollider().center = t.InverseTransformVector(ht.position - hr.position) / footsMul2;
            RightLowerLeg.RagdollCollider().radius = RightLowerLeg.RagdollCollider().height * 0.145f * scale;

            Ragdoll_JointbodyConnect(RightLowerLeg, RightUpperLeg.RagdollBody());
            Ragdoll_Joint(RightLowerLeg, -155f, 1f, 15f, 15f, spring, damper);

            Ragdoll_ComputeAxis(baseTransform, RightLowerLeg, ref f, ref r, ref u);
            Ragdoll_JointAxis(RightLowerLeg, r, null, r, f, u);


            if (generateFists)
            {
                float refLen = Vector3.Distance(RightHand.position, RightForearm.position) * 0.35f;
                ht = RightForearm; hr = RightUpperArm;
                refLen *= scale;

                BoxCollider footBox = AddIfDontHave<BoxCollider>(RightHand);

                float firstHeight = refLen * 0.21f;
                float firstWidth = refLen * 0.8f;
                float fistLen = refLen * 0.8f;

                footBox.size =
                    footBox.transform.InverseTransformVector(baseTransform.right * fistLen) * 2f
                    + footBox.transform.InverseTransformVector(baseTransform.up * firstHeight) * 2f
                    + footBox.transform.InverseTransformVector(baseTransform.forward * firstWidth) * 2f
                    ;

                footBox.center = footBox.transform.InverseTransformVector(baseTransform.right * (fistLen * scale))
                    + footBox.transform.InverseTransformVector(baseTransform.forward * (firstWidth * scale * 0.8f));

                Ragdoll_RefreshComponents(characterJoints, RightHand, ht.position - hr.position, true, bounciness, damper, drag, aDrag, projection, projAngle, projDistance, enCollision, preProcessing, false);
                RightHand.RagdollBody().mass = .8f * massMul;

                Ragdoll_JointbodyConnect(RightHand, RightForearm.RagdollBody());
                Ragdoll_Joint(RightHand, -155f, 1f, 15f, 15f, spring, damper);

                Ragdoll_ComputeAxis(baseTransform, RightHand, ref f, ref r, ref u);
                Ragdoll_JointAxis(RightHand, r, null, r, f, u);


                // Left fist

                refLen = Vector3.Distance(LeftHand.position, LeftForearm.position) * 0.35f;
                ht = LeftForearm; hr = LeftUpperArm;
                refLen *= scale;

                footBox = AddIfDontHave<BoxCollider>(LeftHand);

                firstHeight = refLen * 0.21f;
                firstWidth = refLen * 0.8f;
                fistLen = refLen * 0.8f;

                footBox.size =
                    footBox.transform.InverseTransformVector(-baseTransform.right * fistLen) * 2f
                    + footBox.transform.InverseTransformVector(baseTransform.up * firstHeight) * 2f
                    + footBox.transform.InverseTransformVector(baseTransform.forward * firstWidth) * 2f
                    ;

                footBox.center = footBox.transform.InverseTransformVector(-baseTransform.right * (fistLen * scale))
                    + footBox.transform.InverseTransformVector(baseTransform.forward * (firstWidth * scale * 0.8f));

                Ragdoll_RefreshComponents(characterJoints, LeftHand, ht.position - hr.position, true, bounciness, damper, drag, aDrag, projection, projAngle, projDistance, enCollision, preProcessing, false);
                LeftHand.RagdollBody().mass = .8f * massMul;

                Ragdoll_JointbodyConnect(LeftHand, LeftForearm.RagdollBody());
                Ragdoll_Joint(LeftHand, -155f, 1f, 15f, 15f, spring, damper);

                Ragdoll_ComputeAxis(baseTransform, LeftHand, ref f, ref r, ref u);
                Ragdoll_JointAxis(LeftHand, r, null, r, f, u);



            }


            if (generateFoots)
            {
                float refLen = Vector3.Distance(RightFoot.position, RightLowerLeg.position) * 0.35f;
                refLen *= scale;

                BoxCollider footBox = AddIfDontHave<BoxCollider>(RightFoot);

                float footHeight = refLen * 0.2f;
                float footWidth = refLen * 0.175f;
                float footLen = refLen * 0.8f;

                footBox.size =
                    footBox.transform.InverseTransformVector(baseTransform.forward * footLen) * 2f
                    + footBox.transform.InverseTransformVector(baseTransform.up * footHeight) * 2f
                    + footBox.transform.InverseTransformVector(baseTransform.right * footWidth) * 2f
                    ;

                footBox.center = footBox.transform.InverseTransformVector(baseTransform.forward * (footLen * scale));

                Ragdoll_RefreshComponents(characterJoints, RightFoot, ht.position - hr.position, true, bounciness, damper, drag, aDrag, projection, projAngle, projDistance, enCollision, preProcessing, false);
                RightFoot.RagdollBody().mass = .8f * massMul;

                Ragdoll_JointbodyConnect(RightFoot, RightLowerLeg.RagdollBody());
                Ragdoll_Joint(RightFoot, -155f, 1f, 15f, 15f, spring, damper);

                Ragdoll_ComputeAxis(baseTransform, RightFoot, ref f, ref r, ref u);
                Ragdoll_JointAxis(RightFoot, r, null, r, f, u);



                // Left foot
                ht = lFoot; hr = LeftLowerLeg;
                refLen = Vector3.Distance(LeftFoot.position, LeftLowerLeg.position) * 0.35f;
                refLen *= scale;

                footBox = AddIfDontHave<BoxCollider>(LeftFoot);

                footHeight = refLen * 0.2f;
                footWidth = refLen * 0.175f;
                footLen = refLen * 0.8f;

                footBox.size =
                    footBox.transform.InverseTransformVector(baseTransform.forward * footLen) * 2f
                    + footBox.transform.InverseTransformVector(baseTransform.up * footHeight) * 2f
                    + footBox.transform.InverseTransformVector(-baseTransform.right * footWidth) * 2f
                    ;

                footBox.center = footBox.transform.InverseTransformVector(baseTransform.forward * (footLen * scale));

                Ragdoll_RefreshComponents(characterJoints, LeftFoot, ht.position - hr.position, true, bounciness, damper, drag, aDrag, projection, projAngle, projDistance, enCollision, preProcessing, false);
                LeftFoot.RagdollBody().mass = .8f * massMul;

                Ragdoll_JointbodyConnect(LeftFoot, LeftLowerLeg.RagdollBody());
                Ragdoll_Joint(LeftFoot, -155f, 1f, 15f, 15f, spring, damper);

                Ragdoll_ComputeAxis(baseTransform, LeftFoot, ref f, ref r, ref u);
                Ragdoll_JointAxis(LeftFoot, r, null, r, f, u);



            }


            foreach (Transform tb in bones)
            {
                if (tb == null) continue;
                Rigidbody rig = tb.GetComponent<Rigidbody>();
                if (rig) rig.interpolation = interpolation;
            }

        }

        internal void OnSceneGUI()
        {
            if (ragdollTweak != tweakRagd.None)
            {
                generateRagdoll = false;
                bool tweakScale = ragdollTweak == tweakRagd.Scale;

                if (useSymmetry)
                    RagdollProcessor._editor_symmetryRef = BaseTransform;
                else
                    RagdollProcessor._editor_symmetryRef = null;


                if (LeftUpperArm.RagdollCollider()) RagdollProcessor.DrawColliderHandles(LeftUpperArm.RagdollCollider(), tweakScale, false, RightUpperArm.RagdollCollider());
                if (RightUpperArm.RagdollCollider()) RagdollProcessor.DrawColliderHandles(RightUpperArm.RagdollCollider(), tweakScale, false, LeftUpperArm.RagdollCollider());
                if (LeftForearm.RagdollCollider()) RagdollProcessor.DrawColliderHandles(LeftForearm.RagdollCollider(), tweakScale, false, RightForearm.RagdollCollider());
                if (RightForearm.RagdollCollider()) RagdollProcessor.DrawColliderHandles(RightForearm.RagdollCollider(), tweakScale, false, LeftForearm.RagdollCollider());

                if (LeftUpperLeg.RagdollCollider()) RagdollProcessor.DrawColliderHandles(LeftUpperLeg.RagdollCollider(), tweakScale, false, RightUpperLeg.RagdollCollider());
                if (LeftLowerLeg.RagdollCollider()) RagdollProcessor.DrawColliderHandles(LeftLowerLeg.RagdollCollider(), tweakScale, false, RightLowerLeg.RagdollCollider());
                if (RightUpperLeg.RagdollCollider()) RagdollProcessor.DrawColliderHandles(RightUpperLeg.RagdollCollider(), tweakScale, false, LeftUpperLeg.RagdollCollider());
                if (RightLowerLeg.RagdollCollider()) RagdollProcessor.DrawColliderHandles(RightLowerLeg.RagdollCollider(), tweakScale, false, LeftLowerLeg.RagdollCollider());

                if (PelvisBone.RagdollCollider()) RagdollProcessor.DrawColliderHandles(PelvisBone.RagdollCollider(), tweakScale, true);
                if (SpineRoot.RagdollCollider()) RagdollProcessor.DrawColliderHandles(SpineRoot.RagdollCollider(), tweakScale, true);
                if (Chest) if (Chest.RagdollCollider()) RagdollProcessor.DrawColliderHandles(Chest.RagdollCollider(), tweakScale, true);
                if (Head.RagdollCollider()) RagdollProcessor.DrawColliderHandles(Head.RagdollCollider(), tweakScale, true);

                if (LeftHand) if (LeftHand.RagdollBCollider()) RagdollProcessor.DrawColliderHandles(LeftHand.RagdollBCollider(), tweakScale, false, RightHand.RagdollBCollider());
                if (RightHand) if (RightHand.RagdollBCollider()) RagdollProcessor.DrawColliderHandles(RightHand.RagdollBCollider(), tweakScale, false, LeftHand.RagdollBCollider());
                if (LeftFoot) if (LeftFoot.RagdollBCollider()) RagdollProcessor.DrawColliderHandles(LeftFoot.RagdollBCollider(), tweakScale, false, RightFoot.RagdollBCollider());
                if (RightFoot) if (RightFoot.RagdollBCollider()) RagdollProcessor.DrawColliderHandles(RightFoot.RagdollBCollider(), tweakScale, false, LeftFoot.RagdollBCollider());

            }

        }


        public List<Transform> bones;

        public Transform PelvisBone;
        public Transform SpineRoot;
        public Transform Chest;
        public Transform Head;
        public Transform LeftUpperArm;
        public Transform RightUpperArm;
        public Transform LeftForearm;
        public Transform RightForearm;
        public Transform LeftUpperLeg;
        public Transform LeftLowerLeg;
        public Transform RightUpperLeg;
        public Transform RightLowerLeg;

        public Transform RightHand;
        public Transform LeftHand;

        public Transform RightFoot;
        public Transform LeftFoot;

        public void SetAllBoneReferences(Transform pelv, Transform spineSt, Transform spineMid, Transform head, Transform lefUpArm, Transform leftForeArm, Transform rightUpArm, Transform rightForearm, Transform leftUpLeg, Transform leftLowLeg, Transform rightUpLeg, Transform rightLowLeg)
        {
            bones = new List<Transform>();

            PelvisBone = pelv;
            SpineRoot = spineSt;
            Chest = spineMid;
            Head = head;
            LeftUpperArm = lefUpArm;
            RightUpperArm = rightUpArm;
            LeftForearm = leftForeArm;
            RightForearm = rightForearm;
            LeftUpperLeg = leftUpLeg;
            LeftLowerLeg = leftLowLeg;
            RightUpperLeg = rightUpLeg;
            RightLowerLeg = rightLowLeg;

            bones.Add(pelv);
            bones.Add(spineSt);
            bones.Add(spineMid);
            bones.Add(head);
            bones.Add(lefUpArm);
            bones.Add(rightUpArm);
            bones.Add(leftForeArm);
            bones.Add(rightForearm);
            bones.Add(leftUpLeg);
            bones.Add(leftLowLeg);
            bones.Add(rightUpLeg);
            bones.Add(rightLowLeg);

            if (generateRagdoll == false)
                if (!generateFists)
                {
                    if (LeftForearm != null) { Transform llc = LeftForearm.GetLimbChild(); if (llc != null) if (llc.GetComponent<Rigidbody>()) generateFists = true; }
                    if (RightForearm != null) { Transform llc = RightForearm.GetLimbChild(); if (llc != null) if (llc.GetComponent<Rigidbody>()) generateFists = true; }
                }

            if (generateRagdoll == false)
                if (!generateFoots)
                {
                    if (LeftLowerLeg != null) { Transform llc = LeftLowerLeg.GetLimbChild(); if (llc != null) if (llc.GetComponent<Rigidbody>()) generateFoots = true; }
                    if (RightLowerLeg != null) { Transform llc = RightLowerLeg.GetLimbChild(); if (llc != null) if (llc.GetComponent<Rigidbody>()) generateFoots = true; }
                }

            if (generateFists)
            {
                LeftHand = LeftForearm.GetLimbChild();
                RightHand = RightForearm.GetLimbChild();
                bones.Add(LeftHand);
                bones.Add(RightHand);
            }

            if (generateFoots)
            {
                LeftFoot = LeftLowerLeg.GetLimbChild();
                RightFoot = RightLowerLeg.GetLimbChild();
                bones.Add(LeftFoot);
                bones.Add(RightFoot);
            }
        }


        public void Ragdoll_JointbodyConnect(Transform bone, Rigidbody connected)
        {
            CharacterJoint charJ = bone.GetComponent<CharacterJoint>();

            if (charJ) charJ.connectedBody = connected;
            else
            {
                ConfigurableJoint confJ = bone.GetComponent<ConfigurableJoint>();
                if (confJ) confJ.connectedBody = connected;
            }
        }

        public void Ragdoll_JointAxis(Transform bone, Vector3 axis, Vector3? swingAxis = null, Vector3? r = null, Vector3? f = null, Vector3? u = null)
        {
            CharacterJoint charJ = bone.GetComponent<CharacterJoint>();

            if (charJ)
            {
                charJ.axis = axis;
                if (swingAxis != null) charJ.swingAxis = swingAxis.Value;
            }
            else
            {
                ConfigurableJoint confJ = bone.GetComponent<ConfigurableJoint>();

                if (confJ)
                {
                    confJ.axis = r.Value;
                    if (swingAxis != null) confJ.secondaryAxis = f.Value;
                }
            }
        }

        public void Ragdoll_Joint(Transform bone, float lowTwist, float hightTwist, float lowSwing, float highSwing, float spring = 0f, float damp = 0f)
        {
            CharacterJoint joint = bone.GetComponent<CharacterJoint>();

            if (joint != null)
            {
                SoftJointLimit lim;

                lim = joint.highTwistLimit;
                lim.limit = hightTwist;
                joint.highTwistLimit = lim;

                lim = joint.lowTwistLimit;
                lim.limit = lowTwist;
                joint.lowTwistLimit = lim;

                lim = joint.swing1Limit;
                lim.limit = lowSwing;
                joint.swing1Limit = lim;

                lim = joint.swing2Limit;
                lim.limit = highSwing;
                joint.swing2Limit = lim;


                if (spring > 0f)
                {
                    SoftJointLimitSpring spr = joint.swingLimitSpring;
                    spr.spring = spring;
                    spr.damper = damp;
                    joint.swingLimitSpring = spr;
                }
            }

            ConfigurableJoint cjoint = bone.GetComponent<ConfigurableJoint>();
            if (cjoint)
            {
                var slim = cjoint.lowAngularXLimit;
                slim.limit = lowTwist;
                cjoint.lowAngularXLimit = slim;

                slim = cjoint.highAngularXLimit;
                slim.limit = hightTwist;
                cjoint.highAngularXLimit = slim;

                slim = cjoint.angularYLimit;
                slim.limit = lowSwing;
                cjoint.angularYLimit = slim;

                slim = cjoint.angularZLimit;
                slim.limit = highSwing;
                cjoint.angularZLimit = slim;
            }

        }


        public void RemoveRagdoll()
        {
            foreach (Transform t in bones)
            {
                if (t == null) continue;
                Ragdoll_RemoveFrom(t);
            }

            foreach (Transform t in bones)
            {
                if (t == null) continue;
                if (t.childCount > 0)
                {
                    Ragdoll_RemoveFrom(t.GetChild(0));
                    Ragdoll_RemoveFrom(t.GetLimbChild());
                }
            }

        }

        public void Ragdoll_RefreshComponents(bool characterJoint, Transform bone, Vector3 towards, bool addJoint = true, float bounciness = 0f, float damper = 0f, float drag = 1, float angDrag = 1f, bool projection = true, float projAngle = 90, float projDistance = 0.05f, bool enCollision = false, bool preProcessing = false, bool capsuleColl = true)
        {
            Rigidbody rig = AddIfDontHave<Rigidbody>(bone);

            ConfigurableJoint confJ = null;
            CharacterJoint charJ = null;

            if (addJoint)
            {
                if (characterJoint)
                {
                    charJ = AddIfDontHave<CharacterJoint>(bone);
                    DestroyIfHave<ConfigurableJoint>(bone);
                }
                else
                {
                    confJ = AddIfDontHave<ConfigurableJoint>(bone);
                    DestroyIfHave<CharacterJoint>(bone);
                }
            }


            rig.angularDrag = drag;
            rig.drag = angDrag;
            rig.interpolation = RigidbodyInterpolation.None;

            Vector3 capsuleDir = bone.transform.InverseTransformVector(towards);
            capsuleDir = Prepare_ChooseDominantAxis(capsuleDir);

            if (capsuleColl)
            {
                CapsuleCollider coll = AddIfDontHave<CapsuleCollider>(bone);
                if (capsuleDir.x > 0.1f || capsuleDir.x < -0.1f) coll.direction = 0;
                if (capsuleDir.y > 0.1f || capsuleDir.y < -0.1f) coll.direction = 1;
                if (capsuleDir.z > 0.1f || capsuleDir.z < -0.1f) coll.direction = 2;
            }

            if (confJ)
            {
                confJ.xMotion = ConfigurableJointMotion.Locked;
                confJ.yMotion = ConfigurableJointMotion.Locked;
                confJ.zMotion = ConfigurableJointMotion.Locked;

                confJ.angularXMotion = ConfigurableJointMotion.Limited;
                confJ.angularYMotion = ConfigurableJointMotion.Limited;
                confJ.angularZMotion = ConfigurableJointMotion.Limited;

                confJ.rotationDriveMode = RotationDriveMode.Slerp;

                var spr = confJ.angularXLimitSpring;
                spr.spring = 1500;
                confJ.angularXLimitSpring = spr;

                var drv = confJ.slerpDrive;
                drv.positionSpring = 1500;
                confJ.slerpDrive = drv;
            }

            if (charJ)
            {
                charJ.swingAxis = capsuleDir;

                charJ.enableProjection = projection;
                charJ.projectionAngle = projAngle;
                charJ.projectionDistance = projDistance;

                charJ.enableCollision = enCollision;
                charJ.enablePreprocessing = preProcessing;

                var sp1 = charJ.swingLimitSpring;
                //sp1.spring = 1500;
                sp1.damper = damper;
                charJ.swingLimitSpring = sp1;

                sp1 = charJ.twistLimitSpring;
                sp1.spring = 1500;
                sp1.damper = damper;
                charJ.twistLimitSpring = sp1;

                var sp2 = charJ.lowTwistLimit;
                sp2.bounciness = bounciness;
                charJ.lowTwistLimit = sp2;

                sp2 = charJ.highTwistLimit;
                sp2.bounciness = bounciness;
                charJ.highTwistLimit = sp2;

                sp2 = charJ.swing1Limit;
                sp2.bounciness = bounciness;
                charJ.swing1Limit = sp2;

                sp2 = charJ.swing2Limit;
                sp2.bounciness = bounciness;
                charJ.swing2Limit = sp2;
            }
        }


        /// <summary>
        /// Choosing vector with largest element value to define rounded axis if base transform is offsetted
        /// </summary>
        public static Vector3 Prepare_ChooseDominantAxis(Vector3 axis)
        {
            Vector3 abs = new Vector3(Mathf.Abs(axis.x), Mathf.Abs(axis.y), Mathf.Abs(axis.z));

            if (abs.x > abs.y)
            {
                if (abs.z > abs.x)
                    return new Vector3(0f, 0f, axis.z > 0f ? 1f : -1f);
                else
                    return new Vector3(axis.x > 0f ? 1f : -1f, 0f, 0f);
            }
            else
                if (abs.z > abs.y) return new Vector3(0f, 0f, axis.z > 0f ? 1f : -1f);
            else
                return new Vector3(0f, axis.y > 0f ? 1f : -1f, 0f);
        }

        public void Ragdoll_RemoveFrom(Transform bone)
        {
            DestroyIfHave<ConfigurableJoint>(bone);
            DestroyIfHave<CharacterJoint>(bone);
            DestroyIfHaveIgnoreTriggers<CapsuleCollider>(bone);
            DestroyIfHaveIgnoreTriggers<SphereCollider>(bone);
            DestroyIfHaveIgnoreTriggers<BoxCollider>(bone);
            DestroyIfHave<Rigidbody>(bone);
        }

        public T AddIfDontHave<T>(Transform owner) where T : Component
        {
            T comp = owner.GetComponent<T>();
            if (comp == null) comp = owner.gameObject.AddComponent<T>();
            return comp;
        }

        public void DestroyIfHave<T>(Transform owner) where T : Component
        {
            if (owner == null) return;
            T comp = owner.GetComponent<T>();
            if (comp != null) GameObject.DestroyImmediate(comp);
        }

        public void DestroyIfHaveIgnoreTriggers<T>(Transform owner) where T : Collider
        {
            if (owner == null) return;
            T comp = owner.GetComponent<T>();
            if (comp != null) if (comp.isTrigger == false) GameObject.DestroyImmediate(comp);
        }
    }
}
