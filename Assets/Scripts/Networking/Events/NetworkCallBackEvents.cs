using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class NetworkCallBackEvents : MonoBehaviour, INetworkRunnerCallbacks
{
    public delegate void PlayerJoined(NetworkRunner runner, PlayerRef player);
    public static PlayerJoined onPlayerJoined;

    public delegate void ConnectedToServer(NetworkRunner runner);
    public static ConnectedToServer onConnectedToServer;

    public delegate void Input(NetworkRunner runner, NetworkInput input);
    public static Input onInput;

    public delegate void SessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList);
    public static SessionListUpdated ListUpdated;
    
    public void OnConnectedToServer(NetworkRunner runner)
    {
        onConnectedToServer?.Invoke(runner);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
       onPlayerJoined?.Invoke(runner, player);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (NetworkPlayer.Local == null)
        {
            return;
        }
        
        onInput?.Invoke(runner, input);
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        ListUpdated?.Invoke(runner, sessionList);
    }


    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("disconnect");
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }


    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}
