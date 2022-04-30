using FIMSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FProceduralAnimation
{
    [System.Serializable]
    public partial class RagdollProcessor
    {
        [Tooltip("With FreeFallRagdoll enabled you should put ragdoll amount to 100% to make character fall on ground.\n\nWith FreeFallRagdoll disabled you can blend ragdoll with animator in few different ways.")]
        public bool FreeFallRagdoll = true;

        // Main Params

        [Tooltip("Constant amount of ragdoll on model.\n\nWith FreeFallRagdoll enabled you should put ragdoll amount to 100% to make character fall on ground.")] [FPD_Suffix(0f, 1f)] public float RagdolledBlend = 1f;
        [Tooltip("How much strength should be used to rotate towards target pose")] [Range(0f, 1f)] public float RotateToPoseForce = 0.75f;
        [Tooltip("Quality of unity physical iterations")] [Range(1, 16)] public int UnitySolverIterations = 14;
        [Tooltip("If you want to smoothly enable partial ragdoll on bone collision")]
        public bool BlendOnCollision = true;

        [Space(4)]
        [Tooltip("Partial ragdoll amount - making arms always ragdolled with certain amount")] [Range(0f, 1f)] public float ConstantArmsRagdoll = 0f;
        [Tooltip("Partial ragdoll amount - making head always ragdolled with certain amount")] [Range(0f, 1f)] public float ConstantHeadRagdoll = 0f;
        [Tooltip("Partial ragdoll amount - making spine always ragdolled with certain amount")] [Range(0f, 1f)] public float ConstantSpineRagdoll = 0f;

        [FPD_Header("Spring and damp only with configurable joints")]
        [Tooltip("Spring applies more power towards target pose")]
        public float ConfigurableSpring = 1650;
        [Tooltip("Damp power for physical joints - can reduce bouncy effect but require more spring")]
        public float ConfigurableDamp = 18;
        [Tooltip("If using Hips Pin feature - spring power for hips")]
        public float HipsPinSpring = 4000;
        [Tooltip("Additional Custom hips offset when using Hips Pin Feature (local/configurable space offset)")]
        public Vector3 HipsPositionOffset = Vector3.zero;
        [Tooltip("Additional Custom hips rotation offset (for animation offset) when using Hips Pin Feature (in eulers -> 0-360)")]
        public Vector3 HipsAxisRotation = Vector3.zero;

        [FPD_Header("More custom settings")]
        [Tooltip("Used only when performing Stad Up animation from ragdolled state!\n\nUsing method User_RepositionRoot() will move model to ragdoll position, it depends on your Stand Up animations - if stand up animation is placed fully in model center, set this slider to zero: then object will be positioned to start in model center\nIf stand up animation origin is placed in foots position (bottom of body) then set slider to 1: so model will be fitted to foot position")] [Range(0f, 1f)] public float StandUpInFootPoint = 0f;
        [Tooltip("How sudden should be auto-blending to ragdoll on collisions when using partial ragdoll")] [Range(0f, 1f)] public float BlendingSpeed = 0.5f;
        [Tooltip("If arms collision should trigger blending for upper spine to keep bones rotation more synced")] public bool SensitiveCollision = false;

        [Space(6)]
        [Tooltip("If this limbs should have different muscle power than others")] [Range(0f, 1f)] public float HeadForce = 1f;
        [Tooltip("If this limbs should have different muscle power than others")] [Range(0f, 1f)] public float SpineForce = 1f;
        [Tooltip("If this limbs should have different muscle power than others")] [Range(0f, 1f)] public float RightArmForce = 1f;
        [Tooltip("If this limbs should have different muscle power than others")] [Range(0f, 1f)] public float LeftArmForce = 1f;
        [Tooltip("If this limbs should have different muscle power than others")] [Range(0f, 1f)] public float LeftLegForce = 1f;
        [Tooltip("If this limbs should have different muscle power than others")] [Range(0f, 1f)] public float RightLegForce = 1f;


        [Tooltip("Use SwitchAllExtendedAnimatorSync() method if you want to switch this feature through code!\n\nTrying to sync shoulder and additional spine bones on ragdoll to make ragdoll animated pose synced more precisely with animator.")]
        public ESyncMode ExtendedAnimatorSync = ESyncMode.None;
        [Tooltip("(Experimental) Enabling feature of keeping pelvis with physics instead of making it kinematic")]
        public bool HipsPin = false;
        [Tooltip("(Experimental) Trying to animate pelvis bone as animator")]
        public bool TryAnimatePelvis = false;


        [Tooltip("Initializing component after few frames")] public bool StartAfterTPose = false;
        [Tooltip("If you use plugin which is resetting automatically some rigidbody settings, you can enable it to avoid glitches")] public bool ResetIsKinematic = false;

        [Tooltip("If you encounter error with skeleton hips placed in foot position of the skeleton")] public bool FixRootInPelvis = false;
        [Tooltip("If some bones of your characters seems to fall without physical power, you can try enabling this")] public bool Calibrate = false;

        [Tooltip("Calling 'DontDestroyOnLoad, useful if you change scenes wich player character and you want to keep the same ragdoll animator")] public bool Persistent = false;
        [Tooltip("If your animator have enabled 'AnimatePhysics' update mode, you should enable it here too")] public bool AnimatePhysics = false;


        [Tooltip("Not blending in additional bones if some self limbs collides with eath other (for example when arm touches leg spine will not be blended in if using SensitiveCollision)")] public bool BodyIgnoreMore = false;

        public bool Initialized { get; private set; } = false;

        //internal bool CaptureAnimator = true;

        public List<Rigidbody> RagdollLimbs { get; private set; }
        public List<Transform> Limbs { get; private set; }
        public List<PosingBone> RagdollSetup { get; private set; }

        bool haveFists = false;
        bool haveFoots = false;
        Transform containerForDummy = null;

        internal void Initialize(MonoBehaviour caller, Transform objectWithAnimator, Transform customRagdollAnimator = null, Transform rootBone = null, Transform containerForDummy = null)
        {
            Initialized = false;
            this.containerForDummy = containerForDummy;
            if (StartAfterTPose) caller.StartCoroutine(DelayedInitialize(caller, objectWithAnimator, customRagdollAnimator, rootBone));
            else Initialization(caller, objectWithAnimator, customRagdollAnimator, rootBone);

            if (ResetIsKinematic) User_SetAllKinematic(false);
        }


        IEnumerator DelayedInitialize(MonoBehaviour caller, Transform objectWithAnimator, Transform customRagdollAnimator = null, Transform rootBone = null)
        {
            yield return null;
            yield return new WaitForFixedUpdate();
            //yield return new WaitForSecondsRealtime(0.1f);
            Initialization(caller, objectWithAnimator, customRagdollAnimator, rootBone);
        }

        void Initialization(MonoBehaviour caller, Transform objectWithAnimator, Transform customRagdollAnimator = null, Transform rootBone = null)
        {

            PrepareRagdollDummy(objectWithAnimator, rootBone);
            CaptureAnimation();

            if (BaseTransform && Pelvis)
            {
                PelvisLocalForward = Pelvis.InverseTransformDirection(BaseTransform.forward);
                PelvisLocalRight = Pelvis.InverseTransformDirection(BaseTransform.right);
            }

            RagdollLimbs = new List<Rigidbody>();
            Limbs = new List<Transform>();

            PosingBone c = posingPelvis;

            while (c != null)
            {
                if (c.rigidbody != null)
                {
                    RagdollLimbs.Add(c.rigidbody);
                    Limbs.Add(c.transform);
                    c.collisions = c.rigidbody.gameObject.AddComponent<RagdollCollisionHelper>().Initialize(this);
                }

                c = c.child;
            }

            if (posingRightFist != null) posingRightFist.rigidbody.GetComponent<RagdollCollisionHelper>().ignores.Add(posingRightUpperLeg.transform);
            if (posingLeftFist != null) posingLeftFist.rigidbody.GetComponent<RagdollCollisionHelper>().ignores.Add(posingLeftUpperLeg.transform);

            posingPelvis.rigidbody.isKinematic = true;
            RefreshPelvisGuides();

            if (customRagdollAnimator)
            {
                SetAnimationRefBones
                (
                FTransformMethods.FindChildByNameInDepth(Pelvis.name, customRagdollAnimator),
                FTransformMethods.FindChildByNameInDepth(SpineStart.name, customRagdollAnimator),
                FTransformMethods.FindChildByNameInDepth(Chest.name, customRagdollAnimator),
                FTransformMethods.FindChildByNameInDepth(Head.name, customRagdollAnimator),

                FTransformMethods.FindChildByNameInDepth(LeftUpperArm.name, customRagdollAnimator),
                FTransformMethods.FindChildByNameInDepth(LeftForeArm.name, customRagdollAnimator),
                FTransformMethods.FindChildByNameInDepth(RightUpperArm.name, customRagdollAnimator),
                FTransformMethods.FindChildByNameInDepth(RightForeArm.name, customRagdollAnimator),

                FTransformMethods.FindChildByNameInDepth(LeftUpperLeg.name, customRagdollAnimator),
                FTransformMethods.FindChildByNameInDepth(LeftLowerLeg.name, customRagdollAnimator),
                FTransformMethods.FindChildByNameInDepth(RightUpperLeg.name, customRagdollAnimator),
                FTransformMethods.FindChildByNameInDepth(RightLowerLeg.name, customRagdollAnimator)
                );
            }

            SwitchAllExtendedAnimatorSync(ExtendedAnimatorSync);

            if (Persistent)
            {
                GameObject.DontDestroyOnLoad(RagdollDummyBase);
            }

            //caller.StartCoroutine(LateFixed());

            RagdollSetup = new List<PosingBone>();
            c = posingPelvis;
            while (c != null)
            {
                RagdollSetup.Add(c);
                c = c.child;
            }

            if (HipsPin) BlendOnCollision = false;

            Initialized = true;
        }


        public void SwitchAllExtendedAnimatorSync(ESyncMode mode)
        {
            PosingBone c = posingPelvis;

            while (c != null)
            {
                c.FullAnimatorSync = mode;
                c = c.child;
            }
        }

        public void CaptureAnimation()
        {
            if (Initialized == false) return;
            pelvisAnimatorPosition = posingPelvis.visibleBone.position;
            pelvisAnimatorLocalPosition = posingPelvis.visibleBone.localPosition;
            pelvisAnimatorRotation = posingPelvis.visibleBone.rotation;
            pelvisAnimatorLocalRotation = posingPelvis.visibleBone.localRotation;

            PosingBone c = posingPelvis;

            if (c.ConfigurableJoint) c.CaptureAnimator();

            if (animator)
                if (animator.enabled)
                {
                    while (c != null)
                    {
                        c.CaptureAnimator();
                        c = c.child;
                    }
                }
        }

        Vector3 pelvisAnimatorPosition;

        Vector3 pelvisAnimatorLocalPosition;
        Quaternion pelvisAnimatorRotation;
        Quaternion pelvisAnimatorLocalRotation;

        float spineCulldown = 0f;
        float chestCulldown = 0f;
        float minChestAnim = 0f;
        float sd_minChestAnim = 0f;
        float minSpineAnim = 0f;
        float sd_minSpineAnim = 0f;
        float larmCulldown = 0f;
        float rarmCulldown = 0f;
        float minRArmsAnim = 0f;
        float lminArmsAnim = 0f;
        float sd_lminArmsAnim = 0f;
        float sd_minRArmsAnim = 0f;
        float headCulldown = 0f;
        float minHeadAnim = 0f;
        float sd_minHeadAnim = 0f;


        float fadeOutSpd = 0.4f;
        float fadeInSpd = 0.08f;
        void AnimateMinBlend(ref float val, ref float sd, float target, float tgtMul = 1f)
        {
            val = Mathf.SmoothDamp(val, target * tgtMul, ref sd, target == 1f ? fadeInSpd : fadeOutSpd, Mathf.Infinity, Time.deltaTime);
        }

        internal void LateUpdate(bool captureAnimation = true)
        {
            if (Initialized == false) return;
            //if (fixedAllow == false) {  }
            //else
            //{
            //    if (captureAnimation) CaptureAnimation();
            //    fixedAllow = false;
            //}

            fadeOutSpd = Mathf.LerpUnclamped(1f, 0.15f, BlendingSpeed);
            fadeInSpd = Mathf.LerpUnclamped(.25f, 0.03f, BlendingSpeed);

            #region Blending ragdoll limbs when collision occurs

            larmCulldown -= Time.deltaTime;
            rarmCulldown -= Time.deltaTime;
            spineCulldown -= Time.deltaTime;
            chestCulldown -= Time.deltaTime;
            headCulldown -= Time.deltaTime;

            bool chestColl = false;

            if (BlendOnCollision)
            {
                //if (posingRightUpperArm.Colliding || posingRightForeArm.Colliding) { AnimateMinBlend(ref minRArmsAnim, ref sd_minRArmsAnim, 1f); }
                //else AnimateMinBlend(ref minRArmsAnim, ref sd_minRArmsAnim, ConstantArmsRagdoll);

                //if (posingLeftUpperArm.Colliding || posingLeftForeArm.Colliding) { AnimateMinBlend(ref lminArmsAnim, ref sd_lminArmsAnim, 1f); }
                //else AnimateMinBlend(ref lminArmsAnim, ref sd_lminArmsAnim, ConstantArmsRagdoll);

                bool occured = false;
                if (posingRightUpperArm.Colliding || posingRightForeArm.Colliding) { rarmCulldown = Time.deltaTime * 8f; occured = true; }
                if (posingLeftUpperArm.Colliding || posingLeftForeArm.Colliding) { larmCulldown = Time.deltaTime * 8f; occured = true; }

                if (SensitiveCollision)
                    if (occured)
                    {
                        chestColl = true;
                        chestCulldown = Time.deltaTime * 3f;
                    }

                AnimateMinBlend(ref minRArmsAnim, ref sd_minRArmsAnim, rarmCulldown > 0f ? 1f : ConstantArmsRagdoll);
                AnimateMinBlend(ref lminArmsAnim, ref sd_lminArmsAnim, larmCulldown > 0f ? 1f : ConstantArmsRagdoll);
            }
            else
            {
                lminArmsAnim = ConstantArmsRagdoll;
                minRArmsAnim = ConstantArmsRagdoll;
                sd_lminArmsAnim = 0f;
                sd_minRArmsAnim = 0f;
            }

            posingLeftUpperArm.internalRagdollBlend = lminArmsAnim;
            posingLeftForeArm.internalRagdollBlend = lminArmsAnim;
            posingRightUpperArm.internalRagdollBlend = minRArmsAnim;
            posingRightForeArm.internalRagdollBlend = minRArmsAnim;

            if (BlendOnCollision)
            {
                if (posingHead.Colliding)
                {
                    headCulldown = Time.deltaTime * 9f;

                    bool self = false;
                    if (BodyIgnoreMore) self = posingHead.CollidingOnlyWithSelf;

                    if (!self) chestColl = true;
                }

                AnimateMinBlend(ref minHeadAnim, ref sd_minHeadAnim, headCulldown > 0f ? 1f : ConstantHeadRagdoll);
            }
            else
            {
                minHeadAnim = ConstantHeadRagdoll;
                sd_minHeadAnim = 0f;
            }

            posingHead.internalRagdollBlend = minHeadAnim;

            if (BlendOnCollision)
            {

                float spineMul = 1f;

                bool coll = false;
                if (posingSpineStart.Colliding) coll = true;
                else if (posingChest != null) if (posingChest.Colliding) coll = true;

                if (coll)
                    if (BodyIgnoreMore)
                    {
                        bool self = false;
                        if (posingChest.Colliding) if (posingChest.CollidingOnlyWithSelf) self = true;
                        if (!self) if (posingSpineStart.Colliding) if (posingSpineStart.CollidingOnlyWithSelf) self = true;
                        if (self) coll = false;
                    }

                if (coll)
                {
                    spineCulldown = Time.deltaTime * 10f;
                    chestCulldown = Time.deltaTime * 10f;
                }
                else
                {
                    if (chestColl)
                    {
                        spineCulldown = Time.deltaTime * 6f;
                        chestCulldown = Time.deltaTime * 6f;
                        spineMul = 0.4f;
                        //AnimateMinBlend(ref minSpineAnim, ref sd_minSpineAnim, .4f);
                        //AnimateMinBlend(ref minChestAnim, ref sd_minChestAnim, 1f);
                    }
                    else
                    {
                        //spineCulldown = Time.deltaTime * 10f;
                        //chestCulldown = Time.deltaTime * 10f;
                    }
                }

                AnimateMinBlend(ref minSpineAnim, ref sd_minSpineAnim, spineCulldown > 0f ? 1f : ConstantSpineRagdoll, spineMul);
                AnimateMinBlend(ref minChestAnim, ref sd_minChestAnim, chestCulldown > 0f ? 1f : ConstantSpineRagdoll);

            }
            else
            {
                minSpineAnim = ConstantSpineRagdoll;
                minChestAnim = ConstantSpineRagdoll;
                sd_minSpineAnim = 0f;
                sd_minChestAnim = 0f;
            }

            posingSpineStart.internalRagdollBlend = minSpineAnim;
            if (posingPelvis.ConfigurableJoint) posingPelvis.internalRagdollBlend = minSpineAnim;
            if (posingChest != null) posingChest.internalRagdollBlend = minChestAnim;

            #endregion

            if (captureAnimation) CaptureAnimation();

            // Animating not jointed ragdoll bones to keep model in sync
            for (int i = 0; i < toReanimateBones.Count; i++)
            {
                toReanimateBones[i].SyncRagdollBone(RagdolledBlend, animator.enabled);
            }

            #region Hard sync on collision


            if (RagdolledBlend > 0f || minRArmsAnim > 0.01f || lminArmsAnim > 0.01f)
            {
                float blend = RagdolledBlend;
                if (minRArmsAnim > 0.01f) blend = Mathf.Max(RagdolledBlend, minRArmsAnim);
                else if (lminArmsAnim > 0.01f) blend = Mathf.Max(RagdolledBlend, lminArmsAnim);

                // Blending shoulders and not ragdolled spine bones
                //if (posingLeftUpperArm.parentFixer == null || posingLeftUpperArm.parentFixer.wasSyncing == false)
                posingLeftUpperArm.visibleBone.parent.localRotation = Quaternion.LerpUnclamped(posingLeftUpperArm.visibleBone.parent.localRotation, posingLeftUpperArm.transform.parent.localRotation, blend);

                //if (posingRightUpperArm.parentFixer == null || posingRightUpperArm.parentFixer.wasSyncing == false)
                posingRightUpperArm.visibleBone.parent.localRotation = Quaternion.LerpUnclamped(posingRightUpperArm.visibleBone.parent.localRotation, posingRightUpperArm.transform.parent.localRotation, blend);
            }

            if (RagdolledBlend > 0f || chestColl || minChestAnim > 0.01f)
            {
                float blend = RagdolledBlend;
                if (chestColl || minChestAnim > 0.01f) blend = Mathf.Max(RagdolledBlend, minChestAnim);

                // Blending not ragdolled spine bones
                if (posingChest != null) if (posingChest.visibleBone) posingChest.visibleBone.GetChild(0).localRotation = Quaternion.LerpUnclamped(posingChest.visibleBone.GetChild(0).localRotation, posingChest.transform.GetChild(0).localRotation, blend);
                if (posingChest != null) if (posingChest.visibleBone) posingChest.visibleBone.parent.localRotation = Quaternion.LerpUnclamped(posingChest.visibleBone.parent.localRotation, posingChest.transform.parent.localRotation, blend);
            }

            if (RagdolledBlend > 0f || minRArmsAnim > 0.01f || lminArmsAnim > 0.01f)
            {
                float blend = RagdolledBlend;
                if (minRArmsAnim > 0.01f) blend = Mathf.Max(RagdolledBlend, minRArmsAnim);
                else if (lminArmsAnim > 0.01f) blend = Mathf.Max(RagdolledBlend, lminArmsAnim);

                // Blending shoulders and not ragdolled spine bones
                if (posingLeftUpperArm.parentFixer != null)
                    if (posingLeftUpperArm.parentFixer.wasSyncing == false)
                    {
                        posingLeftUpperArm.visibleBone.parent.localRotation = Quaternion.LerpUnclamped(posingLeftUpperArm.visibleBone.parent.localRotation, posingLeftUpperArm.transform.parent.localRotation, blend);
                    }

                if (posingRightUpperArm.parentFixer != null)
                    if (posingRightUpperArm.parentFixer.wasSyncing == false)
                    {
                        posingRightUpperArm.visibleBone.parent.localRotation = Quaternion.LerpUnclamped(posingRightUpperArm.visibleBone.parent.localRotation, posingRightUpperArm.transform.parent.localRotation, blend);
                    }
            }

            if (RagdolledBlend > 0f || chestColl || minChestAnim > 0.01f)
            {
                float blend = RagdolledBlend;
                if (chestColl || minChestAnim > 0.01f) blend = Mathf.Max(RagdolledBlend, minChestAnim);

                // Blending ragdolled spine bones on collision
                if (posingSpineStart.transform) posingSpineStart.SyncAnimatorToRagdoll(blend);
                if (posingChest != null) if (posingChest.transform) posingChest.SyncAnimatorToRagdoll(blend);
            }

            #endregion

            PosingBone c = posingPelvis;
            c.visibleBone.transform.position = Vector3.LerpUnclamped(c.visibleBone.transform.position, c.transform.position, RagdolledBlend * c.user_internalRagdollBlend);

            while (c != null)
            {
                float blend = RagdolledBlend * c.user_internalRagdollBlend;
                if (blend < c.internalRagdollBlend) blend = c.internalRagdollBlend;

                if (c.visibleBone) c.visibleBone.localRotation = Quaternion.LerpUnclamped(c.visibleBone.localRotation, c.transform.localRotation, blend);

                c = c.child;
            }

            #region Freefall switch

            if (lastFreeFall != FreeFallRagdoll)
            {
                lastFreeFall = FreeFallRagdoll;
                c = posingPelvis;

                if (FreeFallRagdoll)
                {
                    while (c != null)
                    {
                        c.rigidbody.mass = c.targetMass;
                        c = c.child;
                    }
                }
                else
                {
                    while (c != null)
                    {
                        c.rigidbody.mass = c.targetMass * 0.4f;
                        c = c.child;
                    }
                }
            }

            #endregion

            if (HipsPin == false && FreeFallRagdoll == false) ReposeHipsCalculations();
        }

        float latestHipsBlend = 0f;
        void ReposeHipsCalculations()
        {
            if (posingPelvis.rigidbody.isKinematic)
            {
                float blend = latestHipsBlend;

                if (containerForDummy == BaseTransform)
                {
                    blend = 1f;
                }

                if (FixRootInPelvis)
                {
                    if (TryAnimatePelvis)
                    {
                        posingPelvis.transform.localPosition = posingPelvis.transform.parent.InverseTransformPoint(pelvisAnimatorPosition);
                        posingPelvis.transform.localRotation = pelvisAnimatorLocalRotation;
                    }
                    else
                    {
                        posingPelvis.transform.localPosition = Vector3.LerpUnclamped(posingPelvis.transform.localPosition, posingPelvis.transform.parent.InverseTransformPoint(pelvisAnimatorPosition), blend);
                        posingPelvis.transform.localRotation = Quaternion.LerpUnclamped(posingPelvis.transform.localRotation, (pelvisAnimatorLocalRotation), blend);
                    }
                }
                else
                {
                    if (TryAnimatePelvis)
                    {
                        posingPelvis.transform.localPosition = pelvisAnimatorLocalPosition;
                        posingPelvis.transform.localRotation = pelvisAnimatorLocalRotation;
                    }
                    else
                    {
                        posingPelvis.transform.localPosition = Vector3.LerpUnclamped(posingPelvis.transform.localPosition, pelvisAnimatorLocalPosition, blend);
                        posingPelvis.transform.localRotation = Quaternion.LerpUnclamped(posingPelvis.transform.localRotation, pelvisAnimatorLocalRotation, blend);
                    }
                }
            }
            else
            {
                //posingPelvis.rigidbody.position = Vector3.LerpUnclamped(posingPelvis.rigidbody.position, pelvisAnimatorPosition, blend);
                //posingPelvis.rigidbody.rotation = Quaternion.LerpUnclamped(posingPelvis.rigidbody.rotation, pelvisAnimatorRotation, blend);
            }
        }

        bool UpdatePhysics { get { return AnimatePhysics; } }//{ if (animator == null) return false; if (mecanim) return mecanim.updateMode == AnimatorUpdateMode.AnimatePhysics; else if (legacyAnim) return legacyAnim.animatePhysics; return false; } }
        internal void Update()
        {
            if (Initialized == false) return;
            if (UpdatePhysics) return;
            Calibration();
        }

        private void Calibration()
        {
            if (!Calibrate) return;

            PosingBone c = posingPelvis;

            if (animator)
                if (animator.enabled)
                {
                    while (c != null)
                    {
                        if (c.visibleBone) c.visibleBone.localRotation = c.initialLocalRotation;
                        c = c.child;
                    }
                }
        }


        bool lastFreeFall = true;
        int fixedFrames = 0;
        internal void FixedUpdate() // Sync with physics --------------------------------------
        {
            if (Initialized == false) return;

            if (UpdatePhysics) Calibration();

            if (StartAfterTPose)
            {
                if (fixedFrames < 4)
                {
                    fixedFrames += 1;
                    if (fixedFrames == 4) CaptureAnimation();
                    return;
                }
            }
            else
            {
                if (fixedFrames < 1)
                {
                    fixedFrames += 1;
                    if (fixedFrames == 1) CaptureAnimation();
                    return;
                }
            }

            if (FreeFallRagdoll) // Fall on floor
            {
                posingPelvis.rigidbody.isKinematic = false;
            }
            else // Nonfreefall - connected with animator
            {
                posingPelvis.rigidbody.isKinematic = true;
                RagdollDummyBase.position = ObjectWithAnimator.transform.position;
                RagdollDummyBase.rotation = ObjectWithAnimator.transform.rotation;
            }

            PosingBone c = posingPelvis;

            float blend = 1f - (posingPelvis.user_internalRagdollBlend * RagdolledBlend);
            if (!FreeFallRagdoll)
            {
                if (blend < c.internalRagdollBlend) blend = c.internalRagdollBlend;
            }

            latestHipsBlend = blend;
            //ReposeHipsCalculations();

            posingHead.internalForceMultiplier = HeadForce;
            posingSpineStart.internalForceMultiplier = SpineForce;
            if (posingChest != null) posingChest.internalForceMultiplier = SpineForce;

            posingRightUpperArm.internalForceMultiplier = RightArmForce;
            posingRightForeArm.internalForceMultiplier = RightArmForce;

            posingLeftUpperArm.internalForceMultiplier = LeftArmForce;
            posingLeftForeArm.internalForceMultiplier = LeftArmForce;

            posingLeftUpperLeg.internalForceMultiplier = LeftLegForce;
            posingLeftLowerLeg.internalForceMultiplier = LeftLegForce;

            posingRightUpperLeg.internalForceMultiplier = RightLegForce;
            posingRightLowerLeg.internalForceMultiplier = RightLegForce;

            c.PositionAlign = false;

            if (!HipsPin)
            {
                if (!FreeFallRagdoll)
                {
                    posingPelvis.rigidbody.useGravity = false;
                    posingPelvis.rigidbody.isKinematic = true;
                }
            }
            else // Hips pin
            {
                if (c.ConfigurableJoint && HipsPin)
                {

                    if (FreeFallRagdoll == false)
                    {
                        posingPelvis.rigidbody.useGravity = false;
                        posingPelvis.rigidbody.isKinematic = false;
                        c.PositionAlign = HipsPin;
                        c.FixedUpdate();
                    }
                    else
                    {
                        var rootConf = c.ConfigurableJoint.xDrive;
                        rootConf.positionSpring = HipsPinSpring;
                        c.ConfigurableJoint.xDrive = rootConf;

                        if (c.ConfigurableJoint.xDrive.positionSpring > 0f)
                        {
                            var pdr = c.ConfigurableJoint.xDrive;
                            pdr.positionSpring = 0;
                            pdr.positionDamper = 0;
                            c.ConfigurableJoint.xDrive = pdr;
                            c.ConfigurableJoint.yDrive = pdr;
                            c.ConfigurableJoint.zDrive = pdr;
                            c.ConfigurableJoint.angularXDrive = pdr;
                            c.ConfigurableJoint.angularYZDrive = pdr;
                        }

                        posingPelvis.rigidbody.useGravity = true;
                        posingPelvis.rigidbody.isKinematic = false;
                    }
                }
            }

            c = c.child;
            while (c != null)
            {
                c.FixedUpdate();
                c = c.child;
            }
        }


        // Supporting second solution for fixed animate physics mode
        //private bool lateFixedIsRunning = false;
        //private bool fixedAllow = true;
        //private IEnumerator LateFixed()
        //{
        //    WaitForFixedUpdate fixedWait = new WaitForFixedUpdate();
        //    lateFixedIsRunning = true;

        //    while (true)
        //    {
        //        yield return fixedWait;
        //        fixedAllow = true;
        //    }
        //}


    }
}