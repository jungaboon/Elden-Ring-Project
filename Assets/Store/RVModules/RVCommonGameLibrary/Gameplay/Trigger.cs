// Created by Ronis Vision. All rights reserved
// 05.03.2021.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RVModules.RVCommonGameLibrary.Gameplay
{
    /// <summary>
    /// Simple trigger that will invoke UnityEvent on trigger enter and exit
    /// </summary>
    public class Trigger : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// Objects with those tags will activate this trigger
        /// </summary>
        public List<string> tags = new List<string> {"Untagged"};

        public bool invokeOnlyOnFirstEnter;
        public bool invokeOnlyOnLastExit;

        [Tooltip("How many objects entered trigger")]
        [SerializeField]
        private int entersCount;

        /// <summary>
        /// Called upon entering trigger collider of object that have one of tags from tagsToEnable
        /// </summary>
        public UnityEvent onTriggerEnter;

        /// <summary>
        /// Called upon exiting trigger collider of object that have one of tags from tagsToDisable 
        /// </summary>
        public UnityEvent onTriggerExit;

        #endregion

        #region Public methods

        #endregion

        #region Not public methods

        private void Awake()
        {
            entersCount = 0;
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            foreach (var s in tags)
                if (other.CompareTag(s))
                {
                    entersCount--;
                    if (invokeOnlyOnLastExit && entersCount <= 0)
                    {
                        onTriggerExit.Invoke();
                        return;
                    }

                    if (!invokeOnlyOnLastExit) onTriggerExit.Invoke();
                }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            foreach (var s in tags)
                if (other.CompareTag(s))
                {
                    entersCount++;
                    if (invokeOnlyOnFirstEnter && entersCount > 1) return;
                    onTriggerEnter.Invoke();
                    return;
                }
        }

        #endregion
    }
}