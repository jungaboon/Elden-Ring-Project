// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RVHonorAI
{
    /// <summary>
    /// Activate and deactivate group of AIs upon entering trigger collider on it's game object, can also be used for custom events 
    /// </summary>
    public class AiZone : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// Objects with those tags will activate this zone
        /// </summary>
        public string[] tagsToEnable = {"Player"};

        /// <summary>
        /// Objects with those tags will deactivate this zone 
        /// </summary>
        public string[] tagsToDisable = {"Player"};

        /// <summary>
        /// Called upon entering trigger collider of object that have one of tags from tagsToEnable
        /// </summary>
        public UnityEvent onActivation;

        /// <summary>
        /// Called upon exiting trigger collider of object that have one of tags from tagsToDisable 
        /// </summary>
        public UnityEvent onDeactivation;

        [SerializeField]
        private bool active;

        [SerializeField]
        private AiActivationType activationType;

        /// <summary>
        /// Characters to be activated and deactivated by this zone
        /// </summary>
        [SerializeField]
        private List<Character> characters = new List<Character>();

        private Action<Character, bool> activationAction;

        #endregion

        #region Public methods

        /// <summary>
        /// Adds characters to be activated and deactivated by this zone
        /// </summary>
        public void AddCharacters(List<Character> _ais)
        {
            characters.AddRange(_ais);
            SwitchAiActive(_ais, active);
        }

        #endregion

        #region Not public methods

        protected virtual void Awake()
        {
            switch (activationType)
            {
                case AiActivationType.DisableAi:
                    activationAction = (_ai, _active) => _ai.enabled = _active;
                    break;
                case AiActivationType.DisableGameObject:
                    activationAction = (_ai, _active) => _ai.gameObject.SetActive(_active);
                    break;
                case AiActivationType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            foreach (var s in tagsToDisable)
                if (other.CompareTag(s))
                {
                    SwitchAiActive(characters, false);
                    onDeactivation?.Invoke();
                    return;
                }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            foreach (var s in tagsToEnable)
                if (other.CompareTag(s))
                {
                    SwitchAiActive(characters, true);
                    onActivation?.Invoke();
                    return;
                }
        }

        protected virtual void SwitchAiActive(List<Character> _aiList, bool _activate)
        {
            foreach (var ai in _aiList)
            {
                if (ai == null) continue;
                activationAction(ai, _activate);
            }

            active = _activate;
        }

        #endregion
    }

    [Serializable] internal enum AiActivationType
    {
        DisableAi,
        DisableGameObject,
        None
    }
}