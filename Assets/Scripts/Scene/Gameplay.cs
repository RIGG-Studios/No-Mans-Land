using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Gameplay : ContextBehaviour
{
    [Networked, Capacity(16)]
    public NetworkDictionary<PlayerRef, Player> Players { get; } = new();


    private SpawnPoint[] _spawnPoints;
    private int _lastSpawnPoint = -1;
    
    public struct SpawnRequest
    {
        public Player Player;
        public int Tick;
    }
    
    private List<SpawnRequest> _spawnRequests = new();

    public void Join(Player player)
    {
        if (!HasStateAuthority)
        {
            return;
        }

        PlayerRef playerRef = player.Object.InputAuthority;

        if (Players.ContainsKey(playerRef))
        {
            Debug.Log($"Player {playerRef} already joined");
        }

        Players.Add(playerRef, player);
        SpawnNetworkPlayer(player);
    }

    public void Leave(NetworkBehaviour player)
    {
        if (!HasStateAuthority)
        {
            return;
        }

        if (!Players.ContainsKey(player.Object.InputAuthority))
        {
            return;
        }

        Players.Remove(player.Object.InputAuthority);
    }

    public override void Spawned()
    {
        Context.Gameplay = this;
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        
        int currentTick = Runner.Tick;

        for (int i = _spawnRequests.Count - 1; i >= 0; i--)
        {
            var request = _spawnRequests[i];

            if (request.Tick > currentTick)
            {
                continue;
            }

            _spawnRequests.RemoveAt(i);

            if (request.Player == null || request.Player.Object == null)
            {
                continue; 
            }

            if (Players.ContainsKey(request.Player.Object.InputAuthority) == false)
            {
                continue;
            }

            SpawnNetworkPlayer(request.Player);
        }
    }

    private void SpawnNetworkPlayer(Player player)
    {
        DespawnNetworkPlayer(player);

        var networkPlayer = SpawnNetworkPlayer(player.Object.InputAuthority, player.PlayerPrefab);
        player.AssignNetworkPlayer(networkPlayer);
    }

    private void DespawnNetworkPlayer(Player player)
    {
        if (player.ActivePlayer == null)
        {
            return;
        }
        
        Runner.Despawn(player.ActivePlayer.Object);
        player.ClearNetworkPlayer();
    }


    private NetworkPlayer SpawnNetworkPlayer(PlayerRef inputAuthority, NetworkPlayer playerPrefab)
    {
        if (_spawnPoints == null)
        {
            _spawnPoints = Runner.SimulationUnityScene.FindObjectsOfTypeInOrder<SpawnPoint>();
        }

        _lastSpawnPoint = (_lastSpawnPoint + 1) % _spawnPoints.Length;
        var spawnPoint = _spawnPoints[_lastSpawnPoint].transform;

        return Runner.Spawn(playerPrefab, spawnPoint.position, spawnPoint.rotation, inputAuthority);
    }
}
