using System;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FProceduralAnimation
{

    public partial class RagdollProcessor
    {
        public enum ESyncMode { None, RagdollToAnimator, AnimatorToRagdoll, SyncRagdollWithParentBones }

        public class PosingBone
        {
            /// <summary> Bone transform of ragdoll - hidden influence </summary>
            public Transform transform;
            /// <summary> Bone transform of animator - visible influence </summary>
            public Transform visibleBone;
            /// <summary> Bone transform of custom pose ragdoll - hidden influence </summary>
            public Transform customRefBone;
            public Transform riggedParent;

            public ToAnimateBone parentFixer;

            /// <summary> Tries to correct arms rotation with shoulder rotation etc. </summary>
            public ESyncMode FullAnimatorSync = ESyncMode.None;

            public Quaternion animatorLocalRotation;
            public Vector3 animatorLocalPosition;

            public Collider collider;
            public Rigidbody rigidbody;

            public RagdollCollisionHelper collisions;
            public RagdollProcessor owner;
            public PosingBone child;

            public float user_internalMusclePower = 1f;
            public float user_internalRagdollBlend = 1f;
            public float user_internalMuscleMultiplier = 1f;
            public float internalMusclePower = 1f;
            public float internalRagdollBlend = 0f;
            public float targetMass = 3f;

            public ConfigurableJoint ConfigurableJoint { get; private set; }
            public CharacterJoint CharacterJoint { get; private set; }
            public bool Colliding { get { return collisions.EnteredCollisions.Count > 0; } }
            public bool CollidingOnlyWithSelf { get { if (collisions.EnteredSelfCollisions == null) return false; return collisions.EnteredCollisions.Count == collisions.EnteredSelfCollisions.Count; } }

            [NonSerialized] public bool PositionAlign = false;

            internal Quaternion initialParentLocalRotation = Quaternion.identity;
            internal Vector3 initialLocalPosition;
            internal Quaternion initialLocalRotation;
            Quaternion localConvert;
            Quaternion jointAxisConversion;
            Quaternion initialAxisCorrection;
            Rigidbody parentHasRigidbody = null;

            public PosingBone(Transform tr, RagdollProcessor owner, bool requireRigidbody = true)
            {
                // Main ----------------------------------
                transform = tr;

                if (tr == null)
                {
                    UnityEngine.Debug.Log("[Ragdoll Animator] Null Exception! Try assigning 'Root Bone' field with first bone of your skeleton!");
                }

                initialLocalPosition = tr.localPosition;
                initialLocalRotation = tr.localRotation;
                visibleBone = null;
                this.owner = owner;
                animatorLocalRotation = tr.localRotation;

                // Physical Components ----------------------------------
                collider = transform.GetComponent<Collider>();

                rigidbody = transform.GetComponent<Rigidbody>();

                if (rigidbody == null)
                {
                    if (requireRigidbody)
                    {
                        UnityEngine.Debug.Log("[Ragdoll Animator] Error in the ragdoll setup! Try assigning 'Root Bone' field manually (first bone of your skeleton)");
                    }
                }
                else
                {
                    rigidbody.maxAngularVelocity = 15f;
                    //if (rigidbody == null) Debug.Log("[Ragdoll Animator] Bone " + transform.name + " is not having Rigidbody attached to it!");
                    targetMass = rigidbody.mass;
                    JointInit();
                }

                if (tr.parent)
                {
                    parentHasRigidbody = tr.parent.GetComponent<Rigidbody>();
                }
            }

            void JointInit()
            {

                // Joints ----------------------------------
                ConfigurableJoint = rigidbody.gameObject.GetComponent<ConfigurableJoint>();

                if (ConfigurableJoint == null)
                {
                    CharacterJoint = rigidbody.gameObject.GetComponent<CharacterJoint>();
                    if (CharacterJoint) riggedParent = CharacterJoint.connectedBody.transform;
                }
                else
                {
                    riggedParent = ConfigurableJoint.connectedBody == null ? transform.parent : ConfigurableJoint.connectedBody.transform;
                }

                if (riggedParent == null) riggedParent = transform.parent;

                // Joint space preparations ----------------------------------
                if (ConfigurableJoint)
                {
                    localConvert = Quaternion.identity;

                    Vector3 forward = Vector3.Cross(ConfigurableJoint.axis, ConfigurableJoint.secondaryAxis).normalized;
                    Vector3 up = Vector3.Cross(forward, ConfigurableJoint.axis).normalized;

                    if (forward == up)
                    {
                        jointAxisConversion = Quaternion.identity;
                        initialAxisCorrection = initialLocalRotation * Quaternion.identity;
                    }
                    else
                    {
                        Quaternion toJointSpace = Quaternion.LookRotation(forward, up);
                        jointAxisConversion = Quaternion.Inverse(toJointSpace);
                        initialAxisCorrection = initialLocalRotation * toJointSpace;
                    }
                }
            }

            internal void SetVisibleBone(Transform visBone)
            {
                if (visBone) if (visBone.parent) initialParentLocalRotation = visBone.parent.localRotation;
                visibleBone = visBone;
            }

            internal void CaptureAnimator()
            {
                if (parentFixer != null) parentFixer.CaptureAnimation();

                if (customRefBone)
                    animatorLocalRotation = customRefBone.localRotation;
                else
                     if (visibleBone != null)
                    animatorLocalRotation = visibleBone.localRotation;

                if (PositionAlign)
                {
                    if (customRefBone)
                    {
                        animatorLocalPosition = customRefBone.localPosition;
                        animWorldPos = (customRefBone.position);
                    }
                    else
                    {
                        animatorLocalPosition = visibleBone.localPosition;
                        animWorldPos = (visibleBone.position);
                    }

                    //animWorldPos += (owner.BaseTransform.position - animWorldPos);
                }
            }

            Vector3 animWorldPos = Vector3.zero;

            public float internalForceMultiplier = 1f;
            public void FixedUpdate()
            {

                float blend = owner.RotateToPoseForce * internalForceMultiplier * internalMusclePower * user_internalMusclePower;

                // Using configurable joint for ragdoll ---------------------------------------
                if (ConfigurableJoint)
                {
                    rigidbody.solverIterations = owner.UnitySolverIterations;

                    var dr = ConfigurableJoint.slerpDrive;
                    dr.positionSpring = owner.ConfigurableSpring * blend;
                    dr.positionDamper = owner.ConfigurableDamp * blend;
                    dr.maximumForce = owner.ConfigurableSpring;
                    ConfigurableJoint.slerpDrive = dr;

                    ConfigurableJoint.angularXDrive = dr;
                    ConfigurableJoint.angularYZDrive = dr;

                    //float angleDiff = Quaternion.Angle(ToConfigurableSpaceRotation(rigidbody.transform.localRotation), ConfigurableJoint.targetRotation);

                    ConfigurableJoint.targetRotation = ToConfigurableSpaceRotation(animatorLocalRotation);
                    //float angleDiff = Quaternion.Angle(ToConfigurableSpaceRotation(rigidbody.transform.localRotation), ConfigurableJoint.targetRotation);
                    //blend *= Mathf.Lerp(0.5f, 2f, angleDiff * 0.1f);

                    if (PositionAlign)
                    {
                        var pdr = ConfigurableJoint.xDrive;
                        pdr.positionSpring = owner.HipsPinSpring * blend;
                        pdr.positionDamper = owner.ConfigurableDamp * blend;
                        pdr.maximumForce = owner.ConfigurableSpring + owner.HipsPinSpring;
                        ConfigurableJoint.xDrive = pdr;
                        ConfigurableJoint.yDrive = pdr;
                        ConfigurableJoint.zDrive = pdr;
                        //ConfigurableJoint. = animatorLocalPosition;
                        //Matrix4x4 mx = Matrix4x4.Rotate(transform.rotation);
                        //mx = mx.inverse;

                        Transform physParent = transform.parent;
                        Vector3 posMap = physParent.TransformPoint(ConfigurableJoint.connectedAnchor);
                        Quaternion rotMap = physParent.rotation;

                        if (owner.HipsAxisRotation != Vector3.zero) rotMap *= Quaternion.Euler(owner.HipsAxisRotation); //Quaternion.FromToRotation(owner.HipsAxisRotation, ;
                        Matrix4x4 mx = Matrix4x4.TRS(posMap, rotMap, physParent.lossyScale);

                        Vector3 wPos = physParent.TransformPoint(animatorLocalPosition);
                        ConfigurableJoint.targetPosition = mx.inverse.MultiplyPoint(wPos) + owner.HipsPositionOffset;

                        //ConfigurableJoint.targetVelocity = new Vector3(3,1,3) - transform.position;
                        //ConfigurableJoint.targetPosition = WorldToConfigurableSpacePosition(animWorldPos);
                    }

                    return;
                }

                if (blend <= 0f) return;

                // Using character joints ------------------------------------------------------
                Vector3 targetAngular = FEngineering.QToAngularVelocity(rigidbody.rotation, transform.parent.rotation * animatorLocalRotation, true);

                if (user_internalMuscleMultiplier != 0f)
                    rigidbody.angularVelocity = Vector3.LerpUnclamped(rigidbody.angularVelocity, targetAngular * user_internalMuscleMultiplier, blend);
                else
                    rigidbody.angularVelocity = Vector3.LerpUnclamped(rigidbody.angularVelocity, targetAngular, blend);

            }


            Quaternion ToConfigurableSpaceRotation(Quaternion local)
            {
                return jointAxisConversion * Quaternion.Inverse(local * localConvert) * initialAxisCorrection;
            }

            internal void SyncAnimatorToRagdoll(float blend)
            {
                if (visibleBone == null) return;
                visibleBone.localRotation = Quaternion.LerpUnclamped(visibleBone.localRotation, transform.localRotation, blend);
            }
        }

        public Transform GetRagdollDummyBoneByAnimatorBone(Transform tr)
        {
            PosingBone p = posingPelvis;
            while (p != null)
            {
                if (p.visibleBone == tr) return p.transform;
                p = p.child;
            }

            return null;
        }

        private List<ToAnimateBone> toReanimateBones = new List<ToAnimateBone>();

        public Vector3 DebugV3 = Vector3.zero;

        public class ToAnimateBone
        {
            public Transform animatorVisibleBone;
            public Transform dummyBone;
            public PosingBone childRagdollBone;

            /// <summary> When zero then not used </summary>
            public float InternalRagdollToAnimatorOverride = 0f;

            private Quaternion latestAnimatorBoneLocalRot;

            public ToAnimateBone(Transform animatorVisibleBone, Transform ragdollBone, PosingBone child)
            {
                this.animatorVisibleBone = animatorVisibleBone;
                this.dummyBone = ragdollBone;
                childRagdollBone = child;
                childRagdollBone.parentFixer = this;
                latestAnimatorBoneLocalRot = animatorVisibleBone.localRotation;
            }

            public void CaptureAnimation()
            {
                latestAnimatorBoneLocalRot = animatorVisibleBone.localRotation;
            }

            internal bool wasSyncing = false;
            internal void SyncRagdollBone(float ragdolledBlend, bool animatorEnabled)
            {
                wasSyncing = false;

                if (childRagdollBone.FullAnimatorSync == ESyncMode.AnimatorToRagdoll)
                {
                    if (animatorEnabled)
                    {
                        if (ragdolledBlend <= 0f)
                        {
                            dummyBone.localRotation = animatorVisibleBone.localRotation;
                        }
                        else
                        {
                            dummyBone.localRotation =
                                Quaternion.LerpUnclamped(
                                animatorVisibleBone.localRotation,
                                dummyBone.localRotation,
                                ragdolledBlend);
                        }


                        Quaternion rotDiff = animatorVisibleBone.localRotation * Quaternion.Inverse(childRagdollBone.initialParentLocalRotation);
                        childRagdollBone.animatorLocalRotation = childRagdollBone.animatorLocalRotation * (rotDiff);

                        wasSyncing = true;
                    }
                }
                else if (childRagdollBone.FullAnimatorSync == ESyncMode.RagdollToAnimator)
                {
                    if (InternalRagdollToAnimatorOverride > 0f)
                        SyncAnimatorBone(Mathf.Max(InternalRagdollToAnimatorOverride, ragdolledBlend));
                    else
                        SyncAnimatorBone(ragdolledBlend);
                }
                else if (childRagdollBone.FullAnimatorSync == ESyncMode.SyncRagdollWithParentBones)
                {
                    if (animatorEnabled)
                    {
                        dummyBone.localRotation = latestAnimatorBoneLocalRot;

                        Quaternion rotDiff = animatorVisibleBone.localRotation * Quaternion.Inverse(latestAnimatorBoneLocalRot);
                        childRagdollBone.animatorLocalRotation = childRagdollBone.animatorLocalRotation * (rotDiff);

                        wasSyncing = true;
                    }
                }

                // Useful for getup animations
                if (childRagdollBone.FullAnimatorSync != ESyncMode.RagdollToAnimator)
                    if (InternalRagdollToAnimatorOverride > 0f)
                    {
                        SyncAnimatorBone(InternalRagdollToAnimatorOverride);
                    }
            }

            internal void SyncAnimatorBone(float ragdollBlend)
            {
                wasSyncing = true;

                if (ragdollBlend >= 1f)
                {
                    animatorVisibleBone.localRotation = dummyBone.localRotation;
                }
                else
                {
                    animatorVisibleBone.localRotation = Quaternion.LerpUnclamped(
                        animatorVisibleBone.localRotation, dummyBone.localRotation,
                        ragdollBlend);
                }
            }
        }

    }
}