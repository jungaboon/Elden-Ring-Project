using System.Collections;
using UnityEngine;


namespace FIMSpace.FProceduralAnimation
{
    public partial class RagdollProcessor
    {
        /// <summary>
        /// Setting all ragdoll limbs rigidbodies kinematic or non kinematic
        /// </summary>
        public void User_SetAllKinematic(bool kinematic = true)
        {
            PosingBone c = posingPelvis.child;
            while (c != null)
            {
                if (c.rigidbody) c.rigidbody.isKinematic = kinematic;
                c = c.child;
            }
        }

        /// <summary>
        /// Setting all ragdoll limbs rigidbodies interpolation mode
        /// </summary>
        public void User_SetAllIterpolation(RigidbodyInterpolation interpolation)
        {
            foreach (var r in RagdollLimbs)
            {
                r.interpolation = interpolation;
            }
        }

        /// <summary>
        /// Adding physical push impact to single rigidbody limb
        /// </summary>
        /// <param name="limb"> Access 'Parameters' for ragdoll limb </param>
        /// <param name="powerDirection"> World space direction vector </param>
        /// <param name="duration"> Time in seconds </param>
        public static IEnumerator User_SetPhysicalImpact(Rigidbody limb, Vector3 powerDirection, float duration)
        {
            float elapsed = -0.0001f;
            WaitForFixedUpdate fixedWait = new WaitForFixedUpdate();

            while (elapsed < duration)
            {
                limb.AddForce(powerDirection, ForceMode.Impulse);
                elapsed += Time.fixedDeltaTime;
                yield return fixedWait;
            }

            yield break;
        }

        /// <summary>
        /// Adding physical push impact to single rigidbody limb
        /// </summary>
        /// <param name="limb"> Access 'Parameters' for ragdoll limb </param>
        /// <param name="powerDirection"> World space direction vector </param>
        /// <param name="duration"> Time in seconds </param>
        public IEnumerator User_SetLimbImpact(Rigidbody limb, Vector3 powerDirection, float duration)
        {
            yield return RagdollProcessor.User_SetPhysicalImpact(limb, powerDirection, duration);
        }

        /// <summary>
        /// Adding physical push impact to all limbs of the ragdoll
        /// </summary>
        /// <param name="powerDirection"> World space direction vector </param>
        /// <param name="duration"> Time in seconds </param>
        public IEnumerator User_SetPhysicalImpactAll(Vector3 powerDirection, float duration)
        {
            float elapsed = -0.0001f;
            WaitForFixedUpdate fixedWait = new WaitForFixedUpdate();

            while (elapsed < duration)
            {
                PosingBone c = posingPelvis.child;
                while (c != null)
                {
                    if (c.rigidbody) c.rigidbody.AddForce(powerDirection, ForceMode.Impulse);
                    c = c.child;
                }

                elapsed += Time.fixedDeltaTime;

                yield return fixedWait;
            }

            yield break;
        }

        /// <summary>
        /// Enable / disable animator component with delay
        /// </summary>
        public IEnumerator User_SwitchAnimator(Animator animator, bool enable, float delay)
        {
            if (delay > 0f) yield return new WaitForSeconds(delay);

            animator.enabled = enable;
            //CaptureAnimator = capturing;

            yield break;
        }

        /// <summary>
        /// Transitioning all rigidbody muscles power to target value
        /// </summary>
        /// <param name="forcePoseEnd"> Target muscle power </param>
        /// <param name="duration"> Transition duration </param>
        /// <param name="delay"> Delay to start transition </param>
        public IEnumerator User_FadeMuscles(float forcePoseEnd = 0f, float duration = 0.75f, float delay = 0f)
        {
            if (delay > 0f) yield return new WaitForSeconds(delay);

            float startPoseForce = RotateToPoseForce;
            float elapsed = -0.0001f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                if (elapsed > duration) elapsed = duration;
                RotateToPoseForce = Mathf.LerpUnclamped(startPoseForce, forcePoseEnd, elapsed / duration);

                yield return null;
            }

            RotateToPoseForce = forcePoseEnd;

            yield break;
        }

        /// <summary>
        /// Forcing applying rigidbody pose to the animator pose and fading out to zero smoothly
        /// </summary>
        public IEnumerator User_ForceRagdollToAnimatorFor(float duration, float forcingFullDelay = 0.2f)
        {
            for (int i = 0; i < toReanimateBones.Count; i++)
            {
                toReanimateBones[i].InternalRagdollToAnimatorOverride = 1f;
            }

            if (forcingFullDelay > 0f) yield return new WaitForSeconds(forcingFullDelay);

            if (duration > 0f)
            {
                float elapsed = -0.0001f;

                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    if (elapsed > duration) elapsed = duration;
                    float progr = elapsed / duration;

                    for (int i = 0; i < toReanimateBones.Count; i++)
                    {
                        toReanimateBones[i].InternalRagdollToAnimatorOverride = 1f - progr;
                    }

                    yield return null;
                }
            }

            for (int i = 0; i < toReanimateBones.Count; i++)
            {
                toReanimateBones[i].InternalRagdollToAnimatorOverride = 0f;
            }

            yield break;
        }


        /// <summary>
        /// Transitioning ragdoll blend value
        /// </summary>
        public IEnumerator User_FadeRagdolledBlend(float targetBlend = 0f, float duration = 0.75f, float delay = 0f)
        {
            if (delay > 0f) yield return new WaitForSeconds(delay);

            if (duration > 0f)
            {
                float startBlend = RagdolledBlend;
                float elapsed = -0.0001f;

                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    if (elapsed > duration) elapsed = duration;
                    RagdolledBlend = Mathf.LerpUnclamped(startBlend, targetBlend, elapsed / duration);

                    yield return null;
                }
            }

            RagdolledBlend = targetBlend;

            yield break;
        }

        public void User_SetAllLimbsVelocity(Vector3 velocity)
        {
            PosingBone c = posingPelvis.child;
            while (c != null)
            {
                if (c.rigidbody) c.rigidbody.velocity = velocity;
                c = c.child;
            }
        }

        /// <summary>
        /// Computing total velocity of all ragdoll limbs
        /// </summary>
        public Vector3 User_GetAllLimbsVelocity()
        {
            Vector3 velo = Vector3.zero;

            PosingBone c = posingPelvis.child;
            while (c != null)
            {
                if (c.rigidbody) velo += c.rigidbody.velocity;
                c = c.child;
            }

            return velo;
        }

        /// <summary>
        /// Computing velocity of ragdoll spine bones
        /// </summary>
        public Vector3 User_GetSpineLimbsVelocity(bool withChest = false)
        {
            Vector3 velo = Vector3.zero;
            velo += posingPelvis.rigidbody.velocity;
            if (withChest) velo += posingChest.rigidbody.velocity;
            return velo;
        }

        /// Computing angular velocity of ragdoll spine bones
        /// </summary>
        public Vector3 User_GetSpineLimbsAngularVelocity(bool withChest = false)
        {
            Vector3 velo = Vector3.zero;
            velo += posingPelvis.rigidbody.angularVelocity;
            if (withChest) velo += posingChest.rigidbody.angularVelocity;
            return velo;
        }

        /// <summary>
        /// Computing pelvis forward pointing direction in world space
        /// </summary>
        public Vector3 User_PelvisWorldForward()
        {
            return posingPelvis.rigidbody.rotation * PelvisLocalForward;
        }

        public enum EGetUpType
        {
            None, FromBack, FromFacedown
        }


        /// <summary>
        /// Checking state for ragdoll get-up possiblity case
        /// </summary>
        public EGetUpType User_CanGetUp(Vector3? worldUp = null, bool canBeNone = true)
        {
            Vector3 up = worldUp == null ? Vector3.up : worldUp.Value;
            float dot = Vector3.Dot(User_PelvisWorldForward(), up);

            //UnityEngine.Debug.Log(up + " vs " + User_PelvisWorldForward().ToString() +  " DOT = " + dot);

            if (canBeNone)
            {
                if (dot > 0.35f) return EGetUpType.FromBack;
                if (dot < -0.35f) return EGetUpType.FromFacedown;
            }
            else
            {
                if (dot >= 0f) return EGetUpType.FromBack;
                if (dot < 0f) return EGetUpType.FromFacedown;
            }

            return EGetUpType.None;
        }

        /// <summary>
        /// Making pelvis kinematic and anchored to pelvis position
        /// </summary>
        public IEnumerator User_AnchorPelvis(bool anchor = true, float duration = 0f)
        {
            if (duration != 0f)
            {
                Vector3 startPos = posingPelvis.rigidbody.position;
                Quaternion startRot = posingPelvis.rigidbody.rotation;
                float elapsed = -0.0001f;

                while (elapsed < duration)
                {

                    yield return new WaitForFixedUpdate();
                    elapsed += Time.fixedDeltaTime;
                    if (elapsed > duration) elapsed = duration;

                    float blend = (1f - RagdolledBlend) * (elapsed / duration);
                    posingPelvis.rigidbody.position = Vector3.LerpUnclamped(startPos, pelvisAnimatorPosition, blend);
                    posingPelvis.rigidbody.rotation = Quaternion.LerpUnclamped(startRot, pelvisAnimatorRotation, blend);

                }
            }

            if (anchor)
            {
                posingPelvis.transform.localPosition = posingPelvis.visibleBone.localPosition;
                posingPelvis.transform.localRotation = posingPelvis.visibleBone.localRotation;
                //posingPelvis.transform.localPosition = Vector3.LerpUnclamped(posingPelvis.transform.localPosition, posingPelvis.visibleBone.localPosition, blend);
                //posingPelvis.transform.localRotation = Quaternion.LerpUnclamped(posingPelvis.transform.localRotation, posingPelvis.visibleBone.localRotation, blend);
            }

            posingPelvis.rigidbody.isKinematic = anchor;

            yield break;
        }

        /// <summary>
        /// Computing target get-up position for ragdoll controller to fit with current ragdolled position
        /// </summary>
        public Vector3 User_ComputeGetUpPosition()
        {
            Vector3 local = Vector3.LerpUnclamped(Vector3.zero, PelvisToBase, StandUpInFootPoint);
            return posingPelvis.transform.TransformPoint(local);
        }

        /// <summary>
        /// Moving ragdoll controller object to fit with current ragdolled position hips
        /// </summary>
        public void User_RepositionRoot(Transform root = null, Vector3? getUpPosition = null, Vector3? worldUp = null, EGetUpType getupType = EGetUpType.None, LayerMask? snapToGround = null)
        {
            Vector3 up = worldUp == null ? Vector3.up : worldUp.Value;
            if (root == null) root = BaseTransform;

            RagdollDummySkeleton.SetParent(null, true);

            // Fitting main transform in target predicted position for stand up
            if (getUpPosition == null)
            {
                root.position = User_ComputeGetUpPosition();
            }
            else
            {
                root.position = getUpPosition.Value;
            }

            if (snapToGround != null) // Place main model on ground point
            {
                RaycastHit hit;
                if (Physics.Raycast(new Ray(root.position, -up), out hit, PelvisToBase.magnitude, snapToGround.Value, QueryTriggerInteraction.Ignore))
                {
                    root.position = hit.point;
                }
            }

            root.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(posingPelvis.transform.rotation * (getupType == EGetUpType.FromBack ? -PelvisLocalUp : PelvisLocalUp), up), up);

            posingPelvis.visibleBone.position = posingPelvis.transform.position;
            posingPelvis.visibleBone.rotation = posingPelvis.transform.rotation;

            if (FixRootInPelvis)
            {
                if (RagdollDummySkeleton.childCount > 0)
                {
                    Transform c = RagdollDummySkeleton.GetChild(0);
                    Vector3 preChildLoc = c.position;
                    Quaternion preChildRot = c.rotation;

                    RagdollDummySkeleton.transform.position = RootInParent.position;
                    RagdollDummySkeleton.transform.rotation = RootInParent.rotation;

                    c.position = preChildLoc;
                    c.rotation = preChildRot;
                }
            }

            RagdollDummyBase.position = root.position;
            RagdollDummyBase.rotation = root.rotation;

            RagdollDummySkeleton.SetParent(RagdollDummyRoot, true);
        }

        public bool User_IsPelvisKinematic()
        {
            return posingPelvis.rigidbody.isKinematic;
        }

        private void SetPosingParams(PosingBone bone, float muscleAmount = 1f, float muscleMultiplier = 1f, float onRagdoll = 1f)
        {
            bone.user_internalMusclePower = muscleAmount;
            bone.user_internalMuscleMultiplier = muscleMultiplier;
            bone.user_internalRagdollBlend = onRagdoll;
        }

        public void User_SetAllParams(float muscleAmount = 1f, float muscleMultiplier = 1f, float onRagdoll = 1f)
        {
            User_SetArmsParams(muscleAmount, muscleMultiplier, onRagdoll);
            User_SetLegsParams(muscleAmount, muscleMultiplier, onRagdoll);
            User_SetSpineParams(muscleAmount, muscleMultiplier, onRagdoll);
        }

        public void User_SetArmsParams(float muscleAmount = 1f, float muscleMultiplier = 1f, float onRagdoll = 1f)
        {
            User_SetLeftArmParams(muscleAmount, muscleMultiplier, onRagdoll);
            User_SetRightArmParams(muscleAmount, muscleMultiplier, onRagdoll);
        }

        public void User_SetLeftArmParams(float muscleAmount = 1f, float muscleMultiplier = 1f, float onRagdoll = 1f)
        {
            SetPosingParams(posingLeftForeArm, muscleAmount, muscleMultiplier, onRagdoll);
            SetPosingParams(posingLeftUpperArm, muscleAmount, muscleMultiplier, onRagdoll);
        }

        public void User_SetRightArmParams(float muscleAmount = 1f, float muscleMultiplier = 1f, float onRagdoll = 1f)
        {
            SetPosingParams(posingRightUpperArm, muscleAmount, muscleMultiplier, onRagdoll);
            SetPosingParams(posingRightForeArm, muscleAmount, muscleMultiplier, onRagdoll);
        }

        public void User_SetLegsParams(float muscleAmount = 1f, float muscleMultiplier = 1f, float onRagdoll = 1f)
        {
            User_SetLeftLegParams(muscleAmount, muscleMultiplier, onRagdoll);
            User_SetRightLegParams(muscleAmount, muscleMultiplier, onRagdoll);
        }

        public IEnumerator IE_User_FadeLegsParams(float muscleAmount = 1f, float muscleMultiplier = 1f, float onRagdoll = 1f, float duration = 0.5f)
        {
            if (duration != 0f)
            {
                float startM = posingLeftUpperLeg.user_internalMusclePower;
                float startMul = posingLeftUpperLeg.user_internalMuscleMultiplier;
                float startBl = posingLeftUpperLeg.user_internalRagdollBlend;

                float elapsed = -0.0001f;

                while (elapsed < duration)
                {

                    yield return new WaitForFixedUpdate();
                    elapsed += Time.fixedDeltaTime;
                    if (elapsed > duration) elapsed = duration;

                    float blend = (elapsed / duration);
                    User_SetLegsParams(Mathf.Lerp(startM, muscleAmount, blend), Mathf.Lerp(startMul, muscleMultiplier, blend), Mathf.Lerp(startBl, onRagdoll, blend));

                }
            }

            User_SetLegsParams(muscleAmount, muscleMultiplier, onRagdoll);

            yield break;
        }

        public IEnumerator IE_User_FadePelvisParams(float muscleAmount = 1f, float muscleMultiplier = 1f, float onRagdoll = 1f, float duration = 0.5f)
        {
            if (duration != 0f)
            {
                float startM = posingPelvis.user_internalMusclePower;
                float startMul = posingPelvis.user_internalMuscleMultiplier;
                float startBl = posingPelvis.user_internalRagdollBlend;

                float elapsed = -0.0001f;

                while (elapsed < duration)
                {

                    yield return new WaitForFixedUpdate();
                    elapsed += Time.fixedDeltaTime;
                    if (elapsed > duration) elapsed = duration;

                    float blend = (elapsed / duration);
                    User_SetPelvisParams(Mathf.Lerp(startM, muscleAmount, blend), Mathf.Lerp(startMul, muscleMultiplier, blend), Mathf.Lerp(startBl, onRagdoll, blend));

                }
            }

            User_SetPelvisParams(muscleAmount, muscleMultiplier, onRagdoll);

            yield break;
        }

        public void User_SetLeftLegParams(float muscleAmount = 1f, float muscleMultiplier = 1f, float onRagdoll = 1f)
        {
            SetPosingParams(posingLeftUpperLeg, muscleAmount, muscleMultiplier, onRagdoll);
            SetPosingParams(posingLeftLowerLeg, muscleAmount, muscleMultiplier, onRagdoll);
        }

        public void User_SetRightLegParams(float muscleAmount = 1f, float muscleMultiplier = 1f, float onRagdoll = 1f)
        {
            SetPosingParams(posingRightUpperLeg, muscleAmount, muscleMultiplier, onRagdoll);
            SetPosingParams(posingRightLowerLeg, muscleAmount, muscleMultiplier, onRagdoll);
        }

        public void User_SetSpineParams(float muscleAmount = 1f, float muscleMultiplier = 1f, float onRagdoll = 1f)
        {
            SetPosingParams(posingChest, muscleAmount, muscleMultiplier, onRagdoll);
            SetPosingParams(posingHead, muscleAmount, muscleMultiplier, onRagdoll);
        }

        public void User_SetPelvisParams(float muscleAmount = 1f, float muscleMultiplier = 1f, float onRagdoll = 1f)
        {
            SetPosingParams(posingPelvis, muscleAmount, muscleMultiplier, onRagdoll);
        }

        public void User_PoseAsInitalPose()
        {
            PosingBone c = posingPelvis.child;
            while (c != null)
            {
                c.transform.localRotation = c.initialLocalRotation;
                if (c.rigidbody) c.rigidbody.rotation = c.transform.rotation;
                c = c.child;
            }
        }

        public void User_PoseAsAnimator()
        {
            PosingBone c = posingPelvis.child;
            while (c != null)
            {
                c.transform.localRotation = c.animatorLocalRotation;
                if (c.rigidbody) c.rigidbody.rotation = c.transform.rotation;
                c = c.child;
            }
        }
    }
}