// Created by Ronis Vision. All rights reserved
// 07.08.2020.

using RVModules.RVUtilities;

namespace RVModules.RVCommonGameLibrary.Network
{
    /// <summary>
    /// Provides network-agnostic way to get INetworkManager object
    /// </summary>
    public class NetworkManagerProvider : MonoSingleton<NetworkManagerProvider>
    {
        private INetworkManager networkManager;

        public INetworkManager NetworkManager => networkManager;

        protected override void SingletonInitialization()
        {
            networkManager = GetComponent<INetworkManager>();
        }
    }
}