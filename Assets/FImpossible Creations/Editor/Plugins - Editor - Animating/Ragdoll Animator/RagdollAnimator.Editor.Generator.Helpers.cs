using FIMSpace.FEditor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FProceduralAnimation
{
    public static class RagdollExtHelpers
    {
        public static Rigidbody RagdollBody(this Transform t)
        {
            return t.GetComponent<Rigidbody>();
        }

        public static CapsuleCollider RagdollCollider(this Transform t)
        {
            return t.GetComponent<CapsuleCollider>();
        }

        public static BoxCollider RagdollBCollider(this Transform t)
        {
            return t.GetComponent<BoxCollider>();
        }


        public static Transform GetLimbChild(this Transform t)
        {
            if (t.childCount == 0) return null;
            if (t.childCount == 1) return t.GetChild(0);

            int targetI = 0;
            float max = float.MinValue;

            for (int i = 0; i < t.childCount; i++)
            {
                Transform ch = t.GetChild(i);
                int allCh = ch.GetComponentsInChildren<Transform>().Length;

                if (allCh > max)
                {
                    max = allCh;
                    targetI = i;
                }
            }

            return t.GetChild(targetI);
        }



    }

}
