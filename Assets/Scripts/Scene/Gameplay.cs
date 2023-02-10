using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

public class Gameplay : ContextBehaviour
{
    [Networked, Capacity(16)]
    public NetworkDictionary<PlayerRef, Player> Players { get; } = new();

    
    [SerializeField] private Backpack backpackPrefab;

    private SpawnPoint[] _spawnPoints;
    private int _lastSpawnPoint = -1;
    
    public struct SpawnRequest
    {
        public Player Player;
        public Vector3 Position;
        public Quaternion Rotation;
        public int Tick;
    }
    
    private List<SpawnRequest> _spawnRequests = new();
    
    public void Join(Player player)
    {
        //only want to execute this code on the server
        if (!HasStateAuthority)
        {
            return;
        }

        PlayerRef playerRef = player.Object.InputAuthority;

        if (Players.ContainsKey(playerRef))
        {
            Debug.Log($"Player {playerRef} already joined");
            return;
        }
        
        
        Context.Teams.AddToTeam(player, out ISpawnPoint spawnPoint);
        Players.Add(playerRef, player);
        
        SpawnNetworkPlayer(player, spawnPoint.Transform.position, spawnPoint.Transform.rotation);
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

            SpawnNetworkPlayer(request.Player, request.Position, request.Rotation);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestSpawn(Player player, Vector3 pos, Quaternion rot, float delay = 0)
    {
        AddSpawnRequest(player, delay, pos, rot);
    }

    public void SpawnNetworkPlayer(Player player, Vector3 pos, Quaternion rot)
    {
        DespawnNetworkPlayer(player);

        NetworkPlayer networkPlayer = SpawnNetworkPlayer(player.Object.InputAuthority, player.PlayerPrefab,  pos, rot);
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

    public void OnPlayerDeath(NetworkHealthHandler health)
    {
        if (health == null)
        {
            return;
        }
        
        if (Players.TryGet(health.Object.InputAuthority, out Player player))
        {
            StartCoroutine(DelayDespawnNetworkPlayer(player));
        }
    }

    private IEnumerator DelayDespawnNetworkPlayer(Player player)
    {
        yield return new WaitForSeconds(3.5f);

        Backpack backpack = Runner.Spawn(backpackPrefab, player.ActivePlayer.transform.position, player.ActivePlayer.transform.rotation);
        backpack.LoadItems(player.PlayerName, player.ActivePlayer.Inventory.Items.ToArray());
        
        player.State = StateTypes.Dead;
        DespawnNetworkPlayer(player);
    }
    
    
    protected void AddSpawnRequest(Player player, float spawnDelay, Vector3 pos = default, Quaternion rot = default)
    {
        int delayTicks = Mathf.RoundToInt(Runner.Simulation.Config.TickRate * spawnDelay);

        _spawnRequests.Add(new SpawnRequest()
        {
            Player = player,
            Position = pos,
            Rotation = rot,
            Tick = Runner.Tick + delayTicks,
        });
    }

    public void Disconnect(NetworkBehaviour player, NetworkRunner runner)
    {
         runner.Shutdown();
         SceneManager.LoadScene(0);

    }

    public void TryFindPlayer(PlayerRef playerRef, out Player player)
    {
        Players.TryGet(playerRef, out player);
    }


    private NetworkPlayer SpawnNetworkPlayer(PlayerRef inputAuthority, NetworkPlayer playerPrefab, Vector3 pos, Quaternion rot)
    {
        return Runner.Spawn(playerPrefab, pos, rot, inputAuthority);
    }
}
