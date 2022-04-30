// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using UnityEngine;

namespace RVModules.RVUtilities.Prototyping
{
    public class CheckTerrainAlphaMapName : ScreenToWorldRaycaster
    {
        #region Not public methods

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                RaycastHit hit;
                if (RaycastCheck(out hit))
                {
                    //Debug.Log(FindObjectOfType<Terrain>().transform.parent.GetComponent<ITerrainTypeGetter>().GetTerrainTypeAtPos(hit.point));
                }
            }
        }

        #endregion
    }
}