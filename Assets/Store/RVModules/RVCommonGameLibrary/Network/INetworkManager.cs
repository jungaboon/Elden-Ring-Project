// Created by Ronis Vision. All rights reserved
// 07.08.2020.

namespace RVModules.RVCommonGameLibrary.Network
{
    public interface INetworkManager
    {
        float NetworkTime { get; }
        bool IsClient { get; }
        bool IsServer { get; }
    }
}