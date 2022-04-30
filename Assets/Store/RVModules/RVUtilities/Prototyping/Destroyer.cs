// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using UnityEngine;

namespace RVModules.RVUtilities.Prototyping
{
    public class Destroyer : MonoBehaviour
    {
        #region Fields

        public int frames;
        private int framesCount;

        #endregion

        #region Not public methods

        private void Update()
        {
            if (framesCount >= frames)
                Destroy(gameObject);
            framesCount++;
        }

        #endregion
    }
}