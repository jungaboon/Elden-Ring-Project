// Created by Ronis Vision. All rights reserved
// 25.08.2020.

using System;
using RVModules.RVLoadBalancer;
using UnityEngine;

namespace RVModules.RVCommonGameLibrary.Gameplay
{
    /// <summary>
    /// Separate logic game object from view
    /// </summary>
    public class GameObjectViewHandler : MonoBehaviour
    {
        #region Fields

//        [SerializeField]
//        [Tooltip("dont assign it, it's created automatically, it's there just for reference")]
        private GameObject viewGameObject;

        [SerializeField]
        [Tooltip("Game objects you want reparented to view game object")]
        private GameObject[] viewGameObjects;

        [SerializeField]
        [Tooltip("Optional interpolate transform to set ViewGameObject as target")]
        private InterpolateTransform interpolateTransform;

        #endregion

        #region Properties

        public GameObject ViewGameObject => viewGameObject;

        #endregion

        #region Not public methods

        private void Start()
        {
            CreateViewObject();
            if (interpolateTransform != null)
            {
                interpolateTransform.InterpolatedTransform = viewGameObject.transform;
                interpolateTransform.enabled = true;
            }
        }

        /// <summary>
        /// Reparents all viewGameObjects to newly game object responsible for interpolating visuals 
        /// </summary>
        protected virtual void CreateViewObject()
        {
            viewGameObject = new GameObject($"{gameObject.name} view");
            ViewGameObject.transform.position = transform.position;
            ViewGameObject.transform.rotation = transform.rotation;
            foreach (var o in viewGameObjects)
            {
                if (o == null) continue;
                var localPos = o.transform.localPosition;
                var localRot = o.transform.localRotation;
                o.transform.SetParent(ViewGameObject.transform, false);
                o.transform.localPosition = localPos;
                o.transform.localRotation = localRot;
            }
        }

        /// <summary>
        /// Sets ViewGameObject active to true
        /// </summary>
        protected void OnEnable()
        {
            if (ViewGameObject != null) ViewGameObject.SetActive(true);
        }

        /// <summary>
        /// Sets ViewGameObject active to false
        /// </summary>
        protected void OnDisable()
        {
            if (ViewGameObject != null) ViewGameObject.SetActive(false);
        }

        /// <summary>
        /// Destroys ViewGameObject
        /// </summary>
        protected virtual void OnDestroy()
        {
            Destroy(ViewGameObject);
        }

        #endregion
    }
}