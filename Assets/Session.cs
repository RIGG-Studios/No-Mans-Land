using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Session : ContextBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private Player playerPrefab;
    

    public enum SessionStates : byte
    {
        WaitingForPlayers,
        StartingGameplay,
        Gameplay,
        EndingGameplay,
        EndingSession
    }
    
    [Networked]
    public SessionStates SessionState { get; set; }

        
    [Networked]
    public TickTimer StartingGameTimer { get; set; }
    
    private Dictionary<PlayerRef, Player> _players = new(16);

    
    public override void Spawned()
    {
        Context.Session = this;
    }
    

    public void PlayerJoined(PlayerRef player)
    {
        if (!Runner.IsServer)
        {
            return;
        }

        Player playerObj = Runner.Spawn(playerPrefab, inputAuthority: player);
        
        _players.Add(player, playerObj);
        Runner.SetPlayerObject(player, playerObj.Object);
        
        CheckForGameStart();
    }

    private void CheckForGameStart()
    {
        int playerCount = Runner.SessionInfo.PlayerCount;
        int maxPlayers = Runner.SessionInfo.MaxPlayers;

        if (playerCount >= maxPlayers)
        {
            SessionState = SessionStates.StartingGameplay;
            StartingGameTimer = TickTimer.CreateFromSeconds(Runner, Context.Config.startingGameTimer);
        }
    }
    

    public override void FixedUpdateNetwork()
    {
        if (SessionState == SessionStates.WaitingForPlayers)
        {
            Context.UI.EnableMenu("WaitingForPlayers");
        }
        else if (SessionState == SessionStates.StartingGameplay)
        {
            Context.UI.EnableMenu("StartingGameplay");

            if (StartingGameTimer.ExpiredOrNotRunning(Runner))
            {
                if (Object.HasStateAuthority)
                {
                    for (int i = 0; i < _players.Count; i++)
                    {
                        _players[i].Context.Gameplay.Join(_players[i]);
                    }
                    
                    SessionState = SessionStates.Gameplay;
                }
                
                Context.UI.CloseAllMenus();
            }
        } 
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

    public float? GetStartingGameTime() => StartingGameTimer.RemainingTime(Runner);
}
