using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkRunnerHandler : MonoBehaviour
{
    [SerializeField] private NetworkRunner networkRunnerPrefab;
    
    private NetworkRunner _networkRunner;


    private void Start()
    {
        _networkRunner = Instantiate(networkRunnerPrefab, Vector3.zero, quaternion.identity);
        _networkRunner.name = "Network Runner";

        var sceneIndex = SceneManager.GetActiveScene().buildIndex;

        var clientTask =
            InitializeRunner(_networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(), sceneIndex, null);
        
        Debug.Log("Server has been started");
    }

    protected virtual Task InitializeRunner(NetworkRunner runner, GameMode gameMode, NetAddress netAddress,
        SceneRef scene, Action<NetworkRunner> initialized)
    {
        var sceneProvider = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        if (sceneProvider == null)
        {
            Debug.Log("Error initializing runner");
            return null;
        }
        
        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address =  netAddress,
            PlayerCount = 16,
            Scene = scene,
            SessionName = "TestRoom",
            Initialized = initialized,
            SceneManager = sceneProvider,
            DisableClientSessionCreation = true
        });
    }
}
