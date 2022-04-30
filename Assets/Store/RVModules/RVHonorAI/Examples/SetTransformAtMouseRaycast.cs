// Created by Ronis Vision. All rights reserved
// 07.02.2021.

using UnityEngine;

namespace RVHonorAI.Examples
{
    public class SetTransformAtMouseRaycast : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private Transform transformToSetPos;

        [SerializeField]
        private LayerMask layerMask;

        [SerializeField]
        private Vector3 positionOffset;

        #endregion

        #region Not public methods

        // Update is called once per frame
        private void Update()
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 2000, layerMask))
                transformToSetPos.position = hit.point + positionOffset;
        }

        #endregion
    }
}