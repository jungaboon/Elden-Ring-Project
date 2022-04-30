// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using UnityEngine;

namespace RVModules.RVUtilities.Prototyping
{
    public class DontDestroy : MonoBehaviour
    {
        #region Not public methods

        private void Awake() => DontDestroyOnLoad(gameObject);

        #endregion
    }
}