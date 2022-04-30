// Created by Ronis Vision. All rights reserved
// 07.08.2020.

using UnityEngine;
using UnityEngine.Events;

namespace RVModules.RVCommonGameLibrary.Network
{
    /// <summary>
    /// Disable/enable game objects depending on wheter theyre local player or not
    /// </summary>
    public class SetupNetworkedPlayer : MonoBehaviour
    {
        #region Fields

        public UnityEvent localPlayer;
        public UnityEvent notLocalPlayer;

        public GameObject[] activateIfLocalGos;
        public Behaviour[] activateIfLocalComponents;

        public GameObject[] deactivateIfLocalGos;
        public Behaviour[] deactivateIfLocalComponents;
        private INetworkObject networkObject;

        #endregion

        #region Not public methods

        private void Awake()
        {
            networkObject = GetComponent<INetworkObject>();
            if (networkObject == null) Debug.LogError("No InetworkObject detected!");
            // default, single playa mode
            Set(true);
        }

        private void Start() => Set(networkObject.IsLocalPlayer);

        private void Set(bool isLocal)
        {
            if (isLocal)
                localPlayer.Invoke();
            else
                notLocalPlayer.Invoke();

            foreach (var comp in activateIfLocalGos) comp.SetActive(isLocal);
            foreach (var comp in deactivateIfLocalGos) comp.SetActive(!isLocal);

            foreach (var comp in activateIfLocalComponents) comp.enabled = isLocal;
            foreach (var comp in deactivateIfLocalComponents) comp.enabled = !isLocal;
        }

        #endregion
    }
}