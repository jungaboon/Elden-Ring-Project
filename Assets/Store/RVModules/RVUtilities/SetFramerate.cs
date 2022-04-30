// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using UnityEngine;

namespace RVModules.RVUtilities
{
    public class SetFramerate : MonoBehaviour
    {
        #region Fields

        public int targetFrameRate = 60;

        #endregion

        #region Not public methods

        // Start is called before the first frame update
        private void Awake() => Application.targetFrameRate = targetFrameRate;

        #endregion
    }
}