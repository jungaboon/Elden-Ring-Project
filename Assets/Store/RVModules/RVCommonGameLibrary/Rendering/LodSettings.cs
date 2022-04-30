// Created by Ronis Vision. All rights reserved
// 07.03.2021.

using RVModules.RVUtilities;
using UnityEngine;

namespace RVModules.RVCommonGameLibrary.Rendering
{
    public class LodSettings : MonoSingleton<LodSettings>
    {
        #region Fields

        [SerializeField]
        private new Camera camera;

        [SerializeField]
        private float lodBias = 1.0f;

        private Transform cameraTransform;

        #endregion

        #region Properties

        public float LodBias
        {
            get => lodBias;
            set => lodBias = value;
        }

        public Camera Camera
        {
            get => camera;
            set
            {
                if (value == null) return;
                camera = value;
                cameraTransform = camera.transform;
            }
        }


        public Transform CameraTransform => cameraTransform;

        #endregion

        #region Not public methods

        protected override void SingletonInitialization()
        {
            if (Camera == null) Camera = Camera.main;
            else cameraTransform = camera.transform;
        }

        #endregion
    }
}