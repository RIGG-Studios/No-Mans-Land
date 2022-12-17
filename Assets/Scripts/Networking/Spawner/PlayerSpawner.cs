using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;
    
    public void OnEnable()
    {
        NetworkCallBackEvents.onPlayerJoined += OnPlayerJoined;
    }

    public void OnDisable()
    {
        NetworkCallBackEvents.onPlayerJoined -= OnPlayerJoined;
    }

    private void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Debug.Log("Spawning player on server");

            Transform spawnPoint = GetRandomSpawnPoint();
            runner.Spawn(playerPrefab, spawnPoint.position, spawnPoint.rotation, player);
        }
        else
        {
            Debug.Log("On Player Joined");
        }
    }

    private Transform GetRandomSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}
