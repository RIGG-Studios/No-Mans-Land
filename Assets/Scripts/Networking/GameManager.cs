using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Editor;
using UnityEngine;


[RequireComponent(typeof(NetworkRunner))]
[RequireComponent(typeof(NetworkCallBackEvents))]
public class GameManager : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private Gameplay gameplayPrefab;
    [SerializeField] private Player playerPrefab;


    private Dictionary<PlayerRef, Player> _players = new(16);
    
    private bool _gameplaySpawned;


    public void PlayerJoined(PlayerRef player)
    {
        if (!Runner.IsServer)
        {
            return;
        }

        if (!_gameplaySpawned)
        {
            Runner.Spawn(gameplayPrefab);
            _gameplaySpawned = true;
        }

        Player playerObj = Runner.Spawn(playerPrefab, inputAuthority: player);
        _players.Add(player, playerObj);
        
        Runner.SetPlayerObject(player, playerObj.Object);
    }

    public void PlayerLeft(PlayerRef playerRef)
    {
        if (!Runner.IsServer)
        {
            return;
        }

        if (!_players.TryGetValue(playerRef, out Player player))
        {
            return;
        }
        
        Runner.Despawn(player.Object);
        _players.Remove(playerRef);
    }
}
