using System;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FProceduralAnimation
{
    public partial class RagdollProcessor
    {
        public class RagdollCollisionHelper : MonoBehaviour
        {
            public bool Colliding = false;
            public bool DebugLogs = false;
            private RagdollProcessor parent = null;

            public RagdollCollisionHelper Initialize(RagdollProcessor owner)
            {
                parent = owner;
                return this;
            }

            [NonSerialized] public List<Transform> EnteredCollisions = new List<Transform>();
            [NonSerialized] public List<Transform> EnteredSelfCollisions = null;
            [NonSerialized] public List<Transform> ignores = new List<Transform>();
            internal bool CollidesJustWithSelf = false;

            private void OnCollisionEnter(Collision collision)
            {
                if (ignores.Contains(collision.transform)) return;
                if (DebugLogs) UnityEngine.Debug.Log(name + " collides with " + collision.transform.name);
                EnteredCollisions.Add(collision.transform);

                if ( parent.BodyIgnoreMore)
                {
                    if (EnteredSelfCollisions == null) EnteredSelfCollisions = new List<Transform>();
                    if ( parent.Limbs.Contains(collision.transform) ) EnteredSelfCollisions.Add(collision.transform);
                }

                Colliding = true;
            }

            private void OnCollisionExit(Collision collision)
            {
                EnteredCollisions.Remove(collision.transform);

                if (parent.BodyIgnoreMore)
                {
                    if (EnteredSelfCollisions == null) EnteredSelfCollisions = new List<Transform>();
                    if (parent.Limbs.Contains(collision.transform)) EnteredSelfCollisions.Remove(collision.transform);
                }

                if (EnteredCollisions.Count == 0) Colliding = false;
            }
        }
    }
}