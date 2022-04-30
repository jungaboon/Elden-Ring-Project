// Created by Ronis Vision. All rights reserved
// 24.08.2020.

using System;
using UnityEngine;
using UnityEngine.Events;

namespace RVModules.RVCommonGameLibrary.Network
{
    /// <summary>
    /// 
    /// </summary>
    public class SetupNetworkedObject : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private bool automaticallySetOnStart = true;

        public NetworkedObjectConfig[] configs =
        {
            new NetworkedObjectConfig(NetworkedObjectConfigType.IsClient),
//            new NetworkedObjectConfig(NetworkedObjectConfigType.IsServer),
//            new NetworkedObjectConfig(NetworkedObjectConfigType.IsHost),
//            new NetworkedObjectConfig(NetworkedObjectConfigType.IsOwner),
//            new NetworkedObjectConfig(NetworkedObjectConfigType.IsPlayer)
        };

        private INetworkObject networkObject;

        #endregion

        #region Not public methods

        private void Awake()
        {
            networkObject = GetComponent<INetworkObject>();
            if (networkObject == null) Debug.LogError("No InetworkObject detected!");
        }

        private void Start()
        {
            if (networkObject == null) return;
            if (automaticallySetOnStart) Set();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Set()
        {
            foreach (var networkedObjectConfig in configs) Set(networkedObjectConfig);
        }

        private void Set(NetworkedObjectConfig config)
        {
            var isTrue = false;

            switch (config.configType)
            {
                case NetworkedObjectConfigType.IsClient:
                    isTrue = networkObject.IsClient;
                    break;
                case NetworkedObjectConfigType.IsServer:
                    isTrue = networkObject.IsServer;
                    break;
                case NetworkedObjectConfigType.IsDedicatedServer:
                    isTrue = networkObject.IsServer && !networkObject.IsClient;
                    break;
                case NetworkedObjectConfigType.IsHost:
                    isTrue = networkObject.IsServer && networkObject.IsClient;
                    break;
                case NetworkedObjectConfigType.IsOwner:
                    isTrue = networkObject.IsOwner;
                    break;
                case NetworkedObjectConfigType.IsLocalPlayer:
                    isTrue = networkObject.IsLocalPlayer;
                    break;
                case NetworkedObjectConfigType.IsConnected:
                    isTrue = networkObject.IsClient || networkObject.IsServer;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (config.isNot) isTrue = !isTrue;

            if (isTrue)
                config.ifTrue.Invoke();
            else
                config.isFalse.Invoke();

            foreach (var comp in config.activateGameObjects) comp.SetActive(isTrue);
            foreach (var comp in config.deactivateGameObjects) comp.SetActive(!isTrue);

            foreach (var comp in config.activateBehaviours) comp.enabled = isTrue;
            foreach (var comp in config.deactivateBehaviours) comp.enabled = !isTrue;
        }

        #endregion
    }

    [Serializable] public class NetworkedObjectConfig
    {
        #region Fields

        //public string name;
        public NetworkedObjectConfigType configType;
        
        [Tooltip("Reverse logic")]
        public bool isNot;

        public GameObject[] activateGameObjects;
        public Behaviour[] activateBehaviours;

        public GameObject[] deactivateGameObjects;
        public Behaviour[] deactivateBehaviours;

        public UnityEvent ifTrue;
        public UnityEvent isFalse;

        #endregion

        public NetworkedObjectConfig(NetworkedObjectConfigType configType)
        {
            this.configType = configType;
        }
    }

    public enum NetworkedObjectConfigType
    {
        IsClient,
        IsServer,
        IsDedicatedServer,
        IsHost,
        IsOwner,
        IsLocalPlayer,

        /// <summary>
        /// Checks if IsClient or IsServer == true
        /// </summary>
        IsConnected
    }
}