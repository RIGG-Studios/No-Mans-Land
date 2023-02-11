using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ConnectionStatus
{
    ConnectingToLobby,
    InLobby,
    Disconnected,
    Connecting,
    Failed,
    Connected
}

public class GameLauncher : MonoBehaviour, INetworkRunnerCallbacks
{
    public static GameLauncher Instance;

    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private NetworkRunner networkRunnerPrefab;
    [SerializeField] private SessionListUIHandler sessionListHandler;
    [SerializeField] private DisconnectionHandler disconnectionHandler;
    [SerializeField] private LoadingScreen loadingScreen;
    
    private NetworkRunner _runner;
    
    public static ConnectionStatus ConnectionStatus = ConnectionStatus.Disconnected;

    private void Start()
    {
        OnJoinLobby();
    }

    public void OnJoinLobby()
    {
        SetConnectionStatus(ConnectionStatus.ConnectingToLobby);
        var clientTask = JoinLobby();
    }

    private async Task JoinLobby()
    {
        string lobbyID = "Lobby";

        var result = await _runner.JoinSessionLobby(SessionLobby.Custom, lobbyID);

        if (!result.Ok)
        {
            Debug.LogError($"Unable to join lobby {lobbyID}");
            SetConnectionStatus(ConnectionStatus.Disconnected);
        }
        else
        {
            Debug.Log("JoinLobby ok");
            SetConnectionStatus(ConnectionStatus.InLobby);
        }
    }

    private void Awake()
    {
        Instance = this;
        
        SpawnRunner();
    }

    private void SpawnRunner()
    {
        _runner = Instantiate(networkRunnerPrefab, Vector3.zero, Quaternion.identity);
        _runner.AddCallbacks(this);
        _runner.name = "Network Runner";
    }


    private void Update()
    {
        
    }

    public bool CreateRunner(GameMode gameMode, string sessionName, string sceneName = "")
    {
        if (_runner == null)
        {
            SpawnRunner();
        }

        var sceneIndex = SceneUtility.GetBuildIndexByScenePath($"scenes/{sceneName}");

        var clientTask =
            InitializeRunner(_runner, gameMode, NetAddress.Any(), sceneIndex, null, sessionName);

        if (clientTask != null)
        {
            SetConnectionStatus(ConnectionStatus.Connecting);
            return true;
        }

        return false;
    }
    
    
    protected virtual Task InitializeRunner(NetworkRunner runner, GameMode gameMode, NetAddress netAddress,
        SceneRef scene, Action<NetworkRunner> initialized, string sessionName)
    {
        var sceneProvider = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        if (sceneProvider == null)
        {
            Debug.Log("Error initializing runner");
            return null;
        }
        
        runner.ProvideInput = true;
        if (loadingScreen != null)
        {
            loadingScreen.Enable();
        }
        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address =  netAddress,
            PlayerCount = gameConfig.maxPlayersPerGame,
            Scene = scene,
            SessionName = sessionName,
            Initialized = initialized,
            SceneManager = sceneProvider,
            DisableClientSessionCreation = true
        });
    }

    private void LeaveSession()
    {
        if (_runner != null)
        {
            _runner.Shutdown();
        }
        else
        {
            SetConnectionStatus(ConnectionStatus.Disconnected);
        }
    }
    
    
    public void SetConnectionStatus(ConnectionStatus status)
    {
        ConnectionStatus = status;
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    //    loadingScreen.Disable();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        SetConnectionStatus(ConnectionStatus.Disconnected);
        
        disconnectionHandler.OnShutdown(shutdownReason);
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        SetConnectionStatus(ConnectionStatus.Connected);
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        SetConnectionStatus(ConnectionStatus.Disconnected);
        LeaveSession();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        SetConnectionStatus(ConnectionStatus.Failed);
        LeaveSession();
        
        disconnectionHandler.OnConnectionFailed(reason);
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<Fusion.SessionInfo> sessionList)
    {
        if (sessionListHandler == null)
        {
            return;
        }

        if (sessionList.Count == 0)
        {
            sessionListHandler.NoSessionsFound();
        }
        else
        {
            sessionListHandler.Clearlist();

            foreach (SessionInfo sessionInfo in sessionList)
            {
                sessionListHandler.AddToList(sessionInfo);
            }
        }
    }
    
    
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }
}
