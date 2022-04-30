// Created by Ronis Vision. All rights reserved
// 07.10.2020.

using UnityEngine;

namespace RVModules.RVUtilities
{
    /// <summary>
    /// Cache unity's slow and often used Time api to make sure its called only once per frame
    /// </summary>
    [DefaultExecutionOrder(-9999)] public class UnityTime : MonoSingleton<UnityTime>
    {
        public static float DeltaTime => Instance.deltaTime;
        public static float Time => Instance.time;

        [SerializeField]
        private float deltaTime;

        [SerializeField]
        private float time;

        private void Update()
        {
            deltaTime = UnityEngine.Time.deltaTime;
            time = UnityEngine.Time.time;
        }
    }
}