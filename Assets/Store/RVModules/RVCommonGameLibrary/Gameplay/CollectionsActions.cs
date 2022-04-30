// Created by Ronis Vision. All rights reserved
// 05.03.2021.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RVModules.RVCommonGameLibrary.Gameplay
{
    /// <summary>
    /// Activate or deactivate array of game objects or behaviours with one method call
    /// </summary>
    public class CollectionsActions : MonoBehaviour
    {
        #region Fields

//        public List<Object> objects = new List<Object>();
        public List<GameObject> gameObjects = new List<GameObject>();
        public List<Behaviour> behaviours = new List<Behaviour>();

        public UnityEvent onEnableBehaviours;
        public UnityEvent onDisableBehaviours;
        public UnityEvent onActivateGameObjects;
        public UnityEvent onDeactivateGameObjects;

        #endregion

        #region Public methods

        public void EnableBehaviours()
        {
            foreach (var behaviour in behaviours)
            {
                if (behaviour == null) continue;
                behaviour.enabled = true;
            }
            onEnableBehaviours.Invoke();
        }

        public void DisableBehaviours()
        {
            foreach (var behaviour in behaviours)
            {
                if (behaviour == null) continue;
                behaviour.enabled = false;
            }
            onDisableBehaviours.Invoke();
        }

        public void ActivateGameObjects()
        {
            foreach (var o in gameObjects)
            {
                if (o == null) continue;
                o.SetActive(true);
            }
            onActivateGameObjects.Invoke();
        }

        public void DeactivateGameObjects()
        {
            foreach (var o in gameObjects)
            {
                if (o == null) continue;
                o.SetActive(false);
            }
            onDeactivateGameObjects.Invoke();
        }

        #endregion
    }
}