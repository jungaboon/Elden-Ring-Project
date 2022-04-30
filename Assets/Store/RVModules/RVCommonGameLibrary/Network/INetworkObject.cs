// Created by Ronis Vision. All rights reserved
// 07.08.2020.

namespace RVModules.RVCommonGameLibrary.Network
{
    public interface INetworkObject
    {
        #region Properties

        bool IsClient { get; }
        bool IsServer { get; }
        bool IsLocalPlayer { get; }
        bool IsOwner { get; }

        #endregion
    }
}