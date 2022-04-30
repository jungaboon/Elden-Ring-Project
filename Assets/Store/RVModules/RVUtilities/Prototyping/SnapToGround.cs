// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using UnityEngine;

namespace RVModules.RVUtilities.Prototyping
{
    public class SnapToGround : MonoBehaviour
    {
        #region Fields

        public LayerMask raymask;

        public Vector3 targetPosOffet;

        public Vector3 startPointOffset;

        #endregion

        #region Not public methods

        // Use this for initialization
        private void Start()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + startPointOffset, Vector3.down, out hit, float.MaxValue, raymask))
                transform.position = hit.point + targetPosOffet;
        }

        #endregion
    }
}