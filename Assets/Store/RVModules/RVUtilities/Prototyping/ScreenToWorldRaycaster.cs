// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using UnityEngine;

namespace RVModules.RVUtilities.Prototyping
{
    public class ScreenToWorldRaycaster
    {
        #region Fields

        public LayerMask mask;

        #endregion

        #region Not public methods

        protected bool RaycastCheck(out RaycastHit hit)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 1000, mask))
                return true;
            return false;
        }

        #endregion
    }
}