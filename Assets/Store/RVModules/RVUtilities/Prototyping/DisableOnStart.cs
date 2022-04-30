// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using UnityEngine;

namespace RVModules.RVUtilities.Prototyping
{
    public class DisableOnStart : MonoBehaviour
    {
        #region Not public methods

        private void Start() => gameObject.SetActive(false);

        #endregion
    }
}