// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RVHonorAI.Utilities
{
    [Serializable] public class RagdollCreator
    {
        #region Fields

        public GameObject ragdollPrefab;
        public Transform sourceRoot;

        private GameObject ragdoll;

        private Transform[] rootTransforms;
        private Transform[] ragdollTransforms;

        private Rigidbody[] rigidbodies;

        #endregion

        #region Public methods

        public void Create() => Create(Vector3.zero, Vector3.one);

        /// <summary>
        /// Applies force to all rigidbodies in forceRadius range. Force will be linearly lower the further it is from forceSourcePoint
        /// Sum of applied forces to all ragdoll rigidbodies will always equals totalForce, regardless of number of rigidbodies and how close to them is source point
        /// </summary>
        public void ApplyPointForce(Vector3 totalForce, Vector3 forceSourcePoint, float forceRadius)
        {
            if (ragdoll == null) return;

            var weights = new List<float>(rigidbodies.Length);

            float weightSum = 0;

            foreach (var rb in rigidbodies)
            {
                var collider = rb.GetComponent<Collider>();
                var rbPos = collider != null ? collider.bounds.center : rb.transform.position;

                //transform.position;
                var hitDist = Vector3.Distance(rbPos, forceSourcePoint);
//                if (hitDist > forceRadius) continue;

//                var actualForce = (rbPos - forceSourcePoint).normalized * force;

                var forceWeight = Mathf.Lerp(.1f, 1, 1 - hitDist / forceRadius);
                weightSum += forceWeight;

                weights.Add(forceWeight);

                //Debug.Log($"f {forcePerRb}, actual f {actualForce.magnitude}, d {hitDist}");
            }

            var percentageOfTotalForceUsed = weightSum / rigidbodies.Length;
            // this is used instead of average, kinda as weighted average
            var forcePerRb = totalForce / percentageOfTotalForceUsed / rigidbodies.Length;

            var appliedForcesSum = Vector3.zero;

            for (var i = 0; i < rigidbodies.Length; i++)
            {
                var rb = rigidbodies[i];
                var appliedForce = weights[i] * forcePerRb;

                rb.AddForce(appliedForce, ForceMode.Impulse);
                appliedForcesSum += appliedForce;
            }

//            Debug.Log($"{percentageOfTotalForceUsed}, total {totalForce.magnitude}, applied {appliedForcesSum.magnitude}");

            // old method, here total force is be dependant on rb count, but its simpler. 
//            foreach (var rb in rigidbodies)
//            {
//                var collider = rb.GetComponent<Collider>();
//                var rbPos = collider != null ? collider.bounds.center : rb.transform.position;
//
//                //transform.position;
//                var hitDist = Vector3.Distance(rbPos, forceSourcePoint);
////                if (hitDist > forceRadius) continue;
//
////                var actualForce = (rbPos - forceSourcePoint).normalized * force;
//
//                var forcePerRb = totalForce;
//                
//                var actualForce = Vector3.Lerp(forcePerRb * .2f, forcePerRb, 1 - (hitDist / forceRadius));
//                Debug.Log($"f {forcePerRb}, actual f {actualForce.magnitude}, d {hitDist}");
//                rb.AddForce(actualForce, ForceMode.Impulse);
//            }
        }

        public GameObject Create(Vector3 _velocity, Vector3 _scale)
        {
            if (ragdoll == null) Initialize();
            if (ragdoll == null) return null;

            ragdoll.transform.SetPositionAndRotation(sourceRoot.position, sourceRoot.rotation);
            ragdoll.transform.localScale = _scale;

            var pos = new List<Vector3>();
            var rot = new List<Quaternion>();
            foreach (var trn in rootTransforms)
            {
                pos.Add(trn.position);
                rot.Add(trn.rotation);
            }

            var i = 0;

            foreach (var ragdollTransform in ragdollTransforms)
            {
                if (i >= pos.Count || i >= rot.Count) continue;
                ragdollTransform.SetPositionAndRotation(pos[i], rot[i]);
                i++;
            }

            ragdoll.SetActive(true);
            foreach (var rigidbody in rigidbodies) rigidbody.velocity += _velocity;
            return ragdoll;
        }

        public void Initialize()
        {
            if (ragdollPrefab == null) return;

            ragdoll = Object.Instantiate(ragdollPrefab, sourceRoot.position, sourceRoot.rotation);
            ragdoll.SetActive(true);

            rootTransforms = sourceRoot.GetComponentsInChildren<Transform>();
            ragdollTransforms = ragdoll.GetComponentsInChildren<Transform>();
            rigidbodies = ragdoll.GetComponentsInChildren<Rigidbody>();

            ragdoll.SetActive(false);
        }

        #endregion
    }
}