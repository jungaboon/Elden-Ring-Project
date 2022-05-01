#if UNITY_EDITOR
using FIMSpace.FEditor;
#endif
using UnityEngine;
using System.Collections;
using System;

namespace FIMSpace.FProceduralAnimation
{
    [AddComponentMenu("FImpossible Creations/Ragdoll Animator")]
    [DefaultExecutionOrder(-1)]
    public class RagdollAnimator : MonoBehaviour
    {
        [HideInInspector] public bool _EditorDrawSetup = true;

        [SerializeField]
        private RagdollProcessor Processor;

        [Tooltip("! REQUIRED ! Just object with Animator and skeleton as child transforms")]
        public Transform ObjectWithAnimator;
        [Tooltip("If null then it will be found automatically - do manual if you encounter some errors after entering playmode")]
        public Transform RootBone;

        [Tooltip("! OPTIONAL ! Leave here nothing to not use the feature! \n\nObject with bones structure to which ragdoll should try fit with it's pose.\nUseful only if you want to animate ragdoll with other animations than the model body animator.")]
        public Transform CustomRagdollAnimator;

        [Tooltip("If generated ragdoll should be destroyed when main skeleton root object stops existing")]
        public bool AutoDestroy = true;

        [HideInInspector] [Tooltip("When false, then ragdoll dummy skeleton will be generated in playmode, when true, it will be generated in edit mode")] 
        public bool PreGenerateDummy = false;

        [Tooltip("Generated ragdoll dummy will be put inside this transform as child object.\n\nAssign main character object for ragdoll to react with character movement rigidbody motion, set other for no motion reaction.")]
        public Transform TargetParentForRagdollDummy;
        public RagdollProcessor Parameters { get { return Processor; } }

        private void Reset()
        {
            if (Processor == null) Processor = new RagdollProcessor();
            Processor.TryAutoFindReferences(transform);
            Animator an = GetComponentInChildren<Animator>();
            if (an) ObjectWithAnimator = an.transform;
        }

        private void Start()
        {
            Processor.Initialize(this, ObjectWithAnimator, CustomRagdollAnimator, RootBone, TargetParentForRagdollDummy);

            if (AutoDestroy)
            {
                if (!Processor.StartAfterTPose) SetAutoDestroy();
                else StartCoroutine(IEAutoDestroyAfterTPose());
            }
        }

        #region Auto Destroy helpers

        IEnumerator IEAutoDestroyAfterTPose()
        {
            while (Parameters.Initialized == false)
            {
                yield return null;
            }

            SetAutoDestroy();
            yield break;
        }

        void SetAutoDestroy()
        {
            autoDestroy = Processor.RagdollDummyBase.gameObject.AddComponent<RagdollAutoDestroy>();
            autoDestroy.Parent = Processor.Pelvis.gameObject;
        }

        #endregion

        private void FixedUpdate()
        {
            Processor.FixedUpdate();
        }

        private void Update()
        {
            Processor.Update();
        }

        private void LateUpdate()
        {
            Processor.LateUpdate();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying == false)
            {
                if (Processor != null)
                    if (Processor._EditorDrawBones)
                        if (Processor.Pelvis && Processor.BaseTransform)
                        {
                            var p = Processor;
                            UnityEditor.Handles.color = new Color(0.3f, 1f, 0.3f, 0.4f);
                            float scaleRef = (p.Pelvis.position - p.BaseTransform.position).magnitude;

                            Gizmos.color = new Color(0.7f, 1f, 0.7f, 0.2f);
                            Gizmos.DrawLine(p.Pelvis.position, p.BaseTransform.position);

                            Gizmos.color = new Color(0.7f, 1f, 0.7f, 0.5f);
                            Gizmos.DrawWireSphere(p.Pelvis.position, scaleRef * 0.03f);
                            Gizmos.DrawWireSphere(p.BaseTransform.position, scaleRef * 0.03f);
                            Color dCol = Gizmos.color;
                            Color dhCol = UnityEditor.Handles.color;

                            if (p.SpineStart)
                            {
                                //Gizmos.DrawLine(p.SpineStart.position, p.Pelvis.position);
                                FGUI_Handles.DrawBoneHandle(p.Pelvis.position, p.SpineStart.position);
                                Gizmos.DrawWireSphere(p.SpineStart.position, scaleRef * 0.02f);

                                if (p.Chest)
                                {
                                    //Gizmos.DrawLine(p.SpineStart.position, p.Chest.position);
                                    FGUI_Handles.DrawBoneHandle(p.SpineStart.position, p.Chest.position);
                                    Gizmos.DrawWireSphere(p.Chest.position, scaleRef * 0.02f);

                                    if (p.Head)
                                    {
                                        FGUI_Handles.DrawBoneHandle(p.Chest.position, p.Head.position);
                                        //Gizmos.DrawLine(p.Head.position, p.Chest.position);
                                    }
                                }
                                else
                                {
                                    if (p.Head)
                                    {
                                        FGUI_Handles.DrawBoneHandle(p.SpineStart.position, p.Head.position);
                                        //Gizmos.DrawLine(p.Head.position, p.SpineStart.position);
                                    }
                                }
                            }

                            if (p.LeftUpperArm)
                            {


                                if (p.Chest)
                                {
                                    bool isPar = false;

                                    if (p.Chest.childCount > 2)
                                    {
                                        if (p.LeftUpperArm.parent == p.Chest) isPar = true;
                                        if (p.LeftUpperArm.parent)
                                        {
                                            if (p.LeftUpperArm.parent.parent == p.Chest) isPar = true;
                                            if (p.LeftUpperArm.parent.parent)
                                            {
                                                if (p.LeftUpperArm.parent.parent.parent == p.Chest) isPar = true;
                                                if (p.LeftUpperArm.parent.parent.parent)
                                                    if (p.LeftUpperArm.parent.parent.parent.parent == p.Chest) isPar = true;
                                            }
                                        }
                                    }

                                    if (!isPar)
                                    {
                                        if (p.Chest.childCount > 2)
                                            UnityEditor.Handles.color = Color.yellow * 0.4f;
                                        else
                                            UnityEditor.Handles.color = Color.yellow * 0.6f;

                                        FGUI_Handles.DrawBoneHandle(p.Chest.position, p.LeftUpperArm.position);
                                        UnityEditor.Handles.Label(p.LeftUpperArm.position, new GUIContent("[!]", "Try assigning parent of shoulders as 'Chest' instead of using '" + p.Chest.name + "'"));
                                    }
                                    else Gizmos.DrawLine(p.Chest.position, p.LeftUpperArm.position);
                                    if (!isPar) UnityEditor.Handles.color = dhCol;

                                }


                                Gizmos.DrawWireSphere(p.LeftUpperArm.position, scaleRef * 0.03f);
                                if (p.LeftForeArm)
                                {
                                    //Gizmos.DrawLine(p.LeftForeArm.position, p.LeftUpperArm.position);
                                    FGUI_Handles.DrawBoneHandle(p.LeftUpperArm.position, p.LeftForeArm.position);
                                    Gizmos.DrawWireSphere(p.LeftForeArm.position, scaleRef * 0.03f);

                                    if (p.LeftForeArm.childCount > 0)
                                    {
                                        Transform ch = p.LeftForeArm.GetChild(0);
                                        //Gizmos.DrawLine(p.LeftForeArm.position, ch.position);
                                        FGUI_Handles.DrawBoneHandle(p.LeftForeArm.position, ch.position);
                                        Gizmos.DrawWireSphere(ch.position, scaleRef * 0.03f);
                                    }
                                }
                            }

                            if (p.RightUpperArm)
                            {


                                if (p.Chest)
                                {
                                    bool isPar = false;

                                    if (p.Chest.childCount > 2)
                                    {
                                        if (p.RightUpperArm.parent == p.Chest) isPar = true;
                                        if (p.RightUpperArm.parent)
                                        {
                                            if (p.RightUpperArm.parent.parent == p.Chest) isPar = true;
                                            if (p.RightUpperArm.parent.parent)
                                            {
                                                if (p.RightUpperArm.parent.parent.parent == p.Chest) isPar = true;
                                                if (p.RightUpperArm.parent.parent.parent)
                                                    if (p.RightUpperArm.parent.parent.parent.parent == p.Chest) isPar = true;
                                            }
                                        }
                                    }

                                    if (!isPar)
                                    {
                                        if (p.Chest.childCount > 2)
                                            UnityEditor.Handles.color = Color.yellow * 0.4f;
                                        else
                                            UnityEditor.Handles.color = Color.yellow * 0.6f;

                                        UnityEditor.Handles.Label(p.RightUpperArm.position, new GUIContent("[!]", "Try assigning parent of shoulders as 'Chest' instead of using '" + p.Chest.name + "'"));
                                        FGUI_Handles.DrawBoneHandle(p.Chest.position, p.RightUpperArm.position);
                                    }
                                    else Gizmos.DrawLine(p.Chest.position, p.RightUpperArm.position);
                                    if (!isPar) UnityEditor.Handles.color = dhCol;

                                }


                                Gizmos.DrawWireSphere(p.RightUpperArm.position, scaleRef * 0.03f);
                                if (p.RightForeArm)
                                {
                                    //Gizmos.DrawLine(p.RightForeArm.position, p.RightUpperArm.position);
                                    FGUI_Handles.DrawBoneHandle(p.RightUpperArm.position, p.RightForeArm.position);
                                    Gizmos.DrawWireSphere(p.RightForeArm.position, scaleRef * 0.03f);
                                    if (p.RightForeArm.childCount > 0)
                                    {
                                        Transform ch = p.RightForeArm.GetChild(0);
                                        //Gizmos.DrawLine(p.RightForeArm.position, ch.position);
                                        FGUI_Handles.DrawBoneHandle(p.RightForeArm.position, ch.position);
                                        Gizmos.DrawWireSphere(ch.position, scaleRef * 0.03f);
                                    }
                                }
                            }

                            if (p.LeftUpperLeg)
                            {

                                if (p.Pelvis.childCount < 3) Gizmos.color = Color.yellow;
                                if (p.Pelvis.childCount < 2) Gizmos.color = Color.red;
                                Gizmos.DrawLine(p.LeftUpperLeg.position, p.Pelvis.position);
                                if (p.Pelvis.childCount < 3) Gizmos.color = dCol;

                                Gizmos.DrawWireSphere(p.LeftUpperLeg.position, scaleRef * 0.03f);
                                if (p.LeftLowerLeg)
                                {
                                    FGUI_Handles.DrawBoneHandle(p.LeftUpperLeg.position, p.LeftLowerLeg.position);
                                    Gizmos.DrawWireSphere(p.LeftLowerLeg.position, scaleRef * 0.03f);
                                    if (p.LeftLowerLeg.childCount > 0)
                                    {
                                        Transform ch = p.LeftLowerLeg.GetChild(0);
                                        FGUI_Handles.DrawBoneHandle(p.LeftLowerLeg.position, ch.position);
                                        Gizmos.DrawWireSphere(ch.position, scaleRef * 0.03f);
                                    }
                                }
                            }

                            if (p.RightUpperLeg)
                            {
                                if (p.Pelvis.childCount < 3) Gizmos.color = Color.yellow;
                                if (p.Pelvis.childCount < 2) Gizmos.color = Color.red;
                                Gizmos.DrawLine(p.RightUpperLeg.position, p.Pelvis.position);
                                if (p.Pelvis.childCount < 3) Gizmos.color = dCol;

                                Gizmos.DrawWireSphere(p.RightUpperLeg.position, scaleRef * 0.03f);
                                if (p.RightLowerLeg)
                                {
                                    FGUI_Handles.DrawBoneHandle(p.RightUpperLeg.position, p.RightLowerLeg.position);
                                    Gizmos.DrawWireSphere(p.RightLowerLeg.position, scaleRef * 0.03f);
                                    if (p.RightLowerLeg.childCount > 0)
                                    {
                                        Transform ch = p.RightLowerLeg.GetChild(0);
                                        FGUI_Handles.DrawBoneHandle(p.RightLowerLeg.position, ch.position);
                                        Gizmos.DrawWireSphere(ch.position, scaleRef * 0.03f);
                                    }
                                }
                            }

                            UnityEditor.Handles.color = Color.white;
                            Gizmos.color = Color.white;
                        }
            }

            Processor.DrawGizmos();
        }

        bool wasDisabled = false;
        private void OnDisable()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
                wasDisabled = true;
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif

                if (wasDisabled)
                {
                    wasDisabled = false;

                    //if (rag.enabled)
                    //{
                    //    rag.enabled = false;
                    //    rag.Parameters.RagdollDummyRoot.gameObject.SetActive(false);
                    //}
                    Parameters.User_PoseAsInitalPose();
                    //rag.enabled = true;
                    Parameters.RagdollDummyRoot.gameObject.SetActive(true);
                    //rag.Parameters.User_PoseAsAnimator();
                }
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                Parameters.SwitchAllExtendedAnimatorSync(Parameters.ExtendedAnimatorSync);
            }
        }
#endif


        // --------------------------------------------------------------------- UTILITIES


        /// <summary>
        /// Adding physical push impact to single rigidbody limb
        /// </summary>
        /// <param name="limb"> Access 'Parameters' for ragdoll limb </param>
        /// <param name="powerDirection"> World space direction vector </param>
        /// <param name="duration"> Time in seconds </param>
        public void User_SetLimbImpact(Rigidbody limb, Vector3 powerDirection, float duration)
        {
            StartCoroutine(Processor.User_SetLimbImpact(limb, powerDirection, duration));
        }

        /// <summary>
        /// Transitioning ragdoll blend value
        /// </summary>
        public void User_EnableFreeRagdoll(float blend = 1f)
        {
            Parameters.FreeFallRagdoll = true;
            User_FadeRagdolledBlend(blend, 0.2f);
        }

        /// <summary>
        /// Adding physical push impact to all limbs of the ragdoll
        /// </summary>
        /// <param name="powerDirection"> World space direction vector </param>
        /// <param name="duration"> Time in seconds </param>
        public void User_SetPhysicalImpactAll(Vector3 powerDirection, float duration)
        {
            StartCoroutine(Processor.User_SetPhysicalImpactAll(powerDirection, duration));
        }

        public void User_SetVelocityAll(Vector3 newVelocity)
        {
            Processor.User_SetAllLimbsVelocity(newVelocity);
        }

        /// <summary>
        /// Enable / disable animator component with delay
        /// </summary>
        public void User_SwitchAnimator(Transform unityAnimator = null, bool enabled = false, float delay = 0f)
        {
            if (unityAnimator == null) unityAnimator = ObjectWithAnimator;
            if (unityAnimator == null) return;

            Animator an = unityAnimator.GetComponent<Animator>();
            if (an)
            {
                StartCoroutine(Processor.User_SwitchAnimator(an, enabled, delay));
            }
        }

        /// <summary>
        /// Triggering different methods which are used in the demo scene for animating getting up from ragdolled state
        /// </summary>
        /// <param name="groundMask"></param>
        public void User_GetUpStack(RagdollProcessor.EGetUpType getUpType, LayerMask groundMask, float targetRagdollBlend = 0f, float targetMusclesPower = 0.85f, float duration = 1.1f)
        {
            StopAllCoroutines();
            User_SwitchAnimator(null, true);
            User_ForceRagdollToAnimatorFor(duration * 0.5f, duration * 0.15f);
            Parameters.FreeFallRagdoll = false;
            User_FadeMuscles(targetMusclesPower, duration, duration * 0.125f);
            User_FadeRagdolledBlend(targetRagdollBlend, duration, duration * 0.125f);
            User_RepositionRoot(null, null, getUpType, groundMask);
        }

        /// <summary>
        /// Transitioning all rigidbody muscles power to target value
        /// </summary>
        /// <param name="forcePoseEnd"> Target muscle power </param>
        /// <param name="duration"> Transition duration </param>
        /// <param name="delay"> Delay to start transition </param>
        public void User_FadeMuscles(float forcePoseEnd = 0f, float duration = 0.75f, float delay = 0f)
        {
            StartCoroutine(Parameters.User_FadeMuscles(forcePoseEnd, duration, delay));
        }

        /// <summary>
        /// Forcing applying rigidbody pose to the animator pose and fading out to zero smoothly
        /// </summary>
        internal void User_ForceRagdollToAnimatorFor(float duration = 1f, float forcingFullDelay = 0.2f)
        {
            StartCoroutine(Parameters.User_ForceRagdollToAnimatorFor(duration, forcingFullDelay));
        }

        /// <summary>
        /// Transitioning ragdoll blend value
        /// </summary>
        public void User_FadeRagdolledBlend(float targetBlend = 0f, float duration = 0.75f, float delay = 0f)
        {
            StartCoroutine(Parameters.User_FadeRagdolledBlend(targetBlend, duration, delay));
        }

        /// <summary>
        /// Setting all ragdoll limbs rigidbodies kinematic or non kinematic
        /// </summary>
        public void User_SetAllKinematic(bool kinematic = true)
        {
            Parameters.User_SetAllKinematic(kinematic);
        }

        /// <summary>
        /// Making pelvis kinematic and anchored to pelvis position
        /// </summary>
        public void User_AnchorPelvis(bool anchor = true, float duration = 0f)
        {
            StartCoroutine(Parameters.User_AnchorPelvis(anchor, duration));
        }

        /// <summary>
        /// Moving ragdoll controller object to fit with current ragdolled position hips
        /// </summary>
        public void User_RepositionRoot(Transform root = null, Vector3? worldUp = null, RagdollProcessor.EGetUpType getupType = RagdollProcessor.EGetUpType.None, LayerMask? snapToGround = null)
        {
            Parameters.User_RepositionRoot(root, null, worldUp, getupType, snapToGround);
        }


        #region Auto Destroy Reference

        private void OnDestroy()
        {
            if (autoDestroy != null) autoDestroy.StartChecking();
        }

        private RagdollAutoDestroy autoDestroy = null;
        private class RagdollAutoDestroy : MonoBehaviour
        {
            public GameObject Parent;
            public void StartChecking() { Check(); if (Parent != null) InvokeRepeating("Check", 0.05f, 0.5f); }
            void Check() { if (Parent == null) Destroy(gameObject); }
        }

        #endregion

    }
}