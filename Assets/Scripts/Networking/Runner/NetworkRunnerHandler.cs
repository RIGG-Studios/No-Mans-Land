using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkRunnerHandler : MonoBehaviour
{
    [SerializeField] private NetworkRunner networkRunnerPrefab;

    private NetworkRunner _networkRunner;

    private void Awake()
    {
        NetworkRunner networkRunnerInScene = FindObjectOfType<NetworkRunner>();

        //Check for existing Network Runner, if existing usen it else instantiatees new one
        if (networkRunnerInScene != null)
        {
            _networkRunner = networkRunnerInScene;
        }
    }
    private void Start()
    {

        if (_networkRunner != null)
        {
            _networkRunner = Instantiate(networkRunnerPrefab, Vector3.zero, quaternion.identity);
            _networkRunner.name = "Network Runner";

            var sceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (SceneManager.GetActiveScene().name != "TestingScene")
            {
                var clientTask =
               InitializeRunner(_networkRunner, GameMode.AutoHostOrClient, GameManager.instance.GetConnectionToken(), "Test Session", NetAddress.Any(), sceneIndex, null);

                Debug.Log(clientTask);
            }



            Debug.Log("Server has been started");
        }

    }

    public void StartHostMigration(HostMigrationToken hostMigrationToken)
    {
        //Create new netwotk runner as old one is being shut down
        _networkRunner = Instantiate(networkRunnerPrefab);
        _networkRunner.name = "Network Runner - Migrated";

        var clientTask =
                InitializeNetworkRunnerHostMigration(_networkRunner, hostMigrationToken);

        Debug.Log("Host Migration Started");
    }
    INetworkSceneManager GetSceneManager(NetworkRunner runner)
    {
        var sceneProvider = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if (sceneProvider == null)
        {
            sceneProvider = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        return sceneProvider;
    }

    protected virtual Task InitializeRunner(NetworkRunner runner, GameMode gameMode, byte[] connectionToken, string sessionName, NetAddress netAddress,
        SceneRef scene, Action<NetworkRunner> initialized)
    {
        var sceneProvider = GetSceneManager(runner);

        if (sceneProvider == null)
        {
            Debug.Log("Error initializing runner");
            return null;
        }

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = netAddress,
            Scene = scene,
            SessionName = sessionName,
            CustomLobbyName = "Our Lobby ID",
            Initialized = initialized,
            SceneManager = sceneProvider,
            ConnectionToken = connectionToken
        });
    }

    protected virtual Task InitializeNetworkRunnerHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        var sceneProvider = GetSceneManager(runner);

        if (sceneProvider == null)
        {
            Debug.Log("Error initializing runner");
            return null;
        }

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            
            /*GameMode = gameMode,
            Address = netAddress,
            Scene = scene,
            SessionName = sessionName,
            CustomLobbyName = "Our Lobby ID",
            Initialized = initialized,*/
            SceneManager = sceneProvider,
            HostMigrationToken = hostMigrationToken, //Contains all the previous info needed to restart runner
            HostMigrationResume = HostMigrationResume, //Invoked to resume simulation
            ConnectionToken = GameManager.instance.GetConnectionToken()
        });
    }

    void HostMigrationResume(NetworkRunner runner)
    {
        Debug.Log($"HostMigrationResume started");

        // Get a reference for each Network object from the old Host
        foreach (var resumeNetworkObject in runner.GetResumeSnapshotNetworkObjects())
        {
            // Grab all the player objects, they have a NetworkCharacterControllerPrototype
            if (resumeNetworkObject.TryGetBehaviour<NetworkCharacterControllerPrototype>(out var characterController))
            {
                runner.Spawn(resumeNetworkObject, position: characterController.ReadPosition(), rotation: characterController.ReadRotation(), onBeforeSpawned: (runner, newNetworkObject) =>
                {
                    newNetworkObject.CopyStateFrom(resumeNetworkObject);

                    /*// Copy info state from old Behaviour to new behaviour
                    if (resumeNetworkObject.TryGetBehaviour<HPHandler>(out HPHandler oldHPHandler))
                    {
                        HPHandler newHPHandler = newNetworkObject.GetComponent<HPHandler>();
                        newHPHandler.CopyStateFrom(oldHPHandler);

                        newHPHandler.skipSettingStartValues = true;
                    }*/

                    //Map the connection token with the new Network player
                    if (resumeNetworkObject.TryGetBehaviour<NetworkPlayer>(out var oldNetworkPlayer))
                    {
                        // Store Player token for reconnection
                        FindObjectOfType<PlayerSpawner>().SetConnectionTokenMapping(oldNetworkPlayer.token, newNetworkObject.GetComponent<NetworkPlayer>());
                    }

                });
            }
        }

        StartCoroutine(CleanUpHostMigrationCO());

        Debug.Log($"HostMigrationResume completed");
    }
    IEnumerator CleanUpHostMigrationCO()
    {
        yield return new WaitForSeconds(5.0f);

        FindObjectOfType<PlayerSpawner>().OnHostMigrationCleanUp();
    }

    public void OnJoinLobby()
    {
        var clientTask = JoinLobby();
    }

    private async Task JoinLobby()
    {
        Debug.Log("JoinLobby started");

        string lobbyID = "OurLobbyID";

        var result = await _networkRunner.JoinSessionLobby(SessionLobby.Custom, lobbyID);

        if (!result.Ok)
        {
            Debug.LogError($"Unable to join lobby {lobbyID}");
        }
        else
        {
            Debug.Log("JoinLobby ok");
        }
    }

    public void CreateGame(string sessionName, string sceneName)
    {
        Debug.Log($"Create session {sessionName} scene {sceneName} build Index {SceneUtility.GetBuildIndexByScenePath($"scenes/{sceneName}")}");

        //Join existing game as a client
        var clientTask = InitializeRunner(_networkRunner, GameMode.Host, GameManager.instance.GetConnectionToken(), sessionName, NetAddress.Any(), SceneUtility.GetBuildIndexByScenePath($"scenes/{sceneName}"), null);

    }

    public void JoinGame(SessionInfo sessionInfo)
    {
        Debug.Log($"Join session {sessionInfo.Name}");

        //Join existing game as a client
        var clientTask = InitializeRunner(_networkRunner, GameMode.Client, GameManager.instance.GetConnectionToken(), sessionInfo.Name, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);

    }
}