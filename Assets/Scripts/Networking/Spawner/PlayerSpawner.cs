using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.Diagnostics;
using Random = UnityEngine.Random;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    Dictionary<int, NetworkPlayer> mapTokenIDWithNetworkPlayer;

    private void Awake()
    {
        mapTokenIDWithNetworkPlayer= new Dictionary<int, NetworkPlayer>();
    }

    public void OnEnable()
    {
        NetworkCallBackEvents.onPlayerJoined += OnPlayerJoined;
    }

    public void OnDisable()
    {
        NetworkCallBackEvents.onPlayerJoined -= OnPlayerJoined;
    }

    int GetPlayerToken(NetworkRunner runner, PlayerRef player)//Will be called in Spawner, used to check if player has joined server before
    {
        if (runner.LocalPlayer == player)
        {
            // Creating token for host and hashing it into an integer token
            return ConnectionTokenUtils.HashToken(GameManager.instance.GetConnectionToken());
        }
        else
        {
            // Connection token for other players
            var token = runner.GetPlayerConnectionToken(player);

            if (token != null)
                return ConnectionTokenUtils.HashToken(token);

            Debug.LogError($"GetPlayerToken returned invalid token");

            return 0; // invalid
        }
    }

    public void SetConnectionTokenMapping(int token, NetworkPlayer networkPlayer)
    {
        mapTokenIDWithNetworkPlayer.Add(token, networkPlayer);
    }

    private void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            //Get the token for the local host player
            int playerToken = GetPlayerToken(runner, player);

            Debug.Log($"Spawning player on server. Connection Token : {playerToken}");

            Transform spawnPoint = GetRandomSpawnPoint();
            runner.Spawn(playerPrefab, spawnPoint.position, spawnPoint.rotation, player);


            //Check if player token has already been generated for this particular server
            if (mapTokenIDWithNetworkPlayer.TryGetValue(playerToken, out NetworkPlayer networkPlayer))
            {
                Debug.Log($"Found old connection token for token {playerToken}. Assigning controlls to that player");

                networkPlayer.GetComponent<NetworkObject>().AssignInputAuthority(player);

                networkPlayer.Spawned();
            }
            else
            {
                Debug.Log($"Spawning new player for connection token {playerToken}");
                NetworkPlayer spawnedNetworkPlayer = runner.Spawn(playerPrefab, spawnPoint.position, spawnPoint.rotation, player).GetComponent<NetworkPlayer>();

                //Storing Network token for the remote player
                spawnedNetworkPlayer.token = playerToken;

                //Storing mapping between playerToken and the spawned network player in global Dictionary
                mapTokenIDWithNetworkPlayer[playerToken] = spawnedNetworkPlayer;
            }
        }
        else
        {
            Debug.Log("On Player Joined");
        }
    }

    public void OnHostMigrationCleanUp()
    {
        Debug.Log("Spawner OnHostMigrationCleanUp started");

        foreach (KeyValuePair<int, NetworkPlayer> entry in mapTokenIDWithNetworkPlayer)
        {
            NetworkObject networkObjectInDictionary = entry.Value.GetComponent<NetworkObject>();

            if (networkObjectInDictionary.InputAuthority.IsNone)
            {
                Debug.Log($"{Time.time} Found player that has not reconnected. Despawning {entry.Value.nickName}");

                networkObjectInDictionary.Runner.Despawn(networkObjectInDictionary);
            }
        }

        Debug.Log("Spawner OnHostMigrationCleanUp completed");
    }

    private Transform GetRandomSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}
