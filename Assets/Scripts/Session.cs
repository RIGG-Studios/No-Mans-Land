    using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
    using NoMansLand.Scene;
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
    
    [Networked(OnChanged = nameof(OnSessionStateChanged), OnChangedTargets = OnChangedTargets.All)]
    public SessionStates SessionState { get; set; }

        
    [Networked]
    public TickTimer StartingGameTimer { get; set; }
    
    [Networked]
    public float GameplayTimer { get; set; }

    [Networked]
    public TickTimer EndingGameplayTimer { get; set; }

    
    private Dictionary<PlayerRef, Player> _players = new(16);

    private static void OnSessionStateChanged(Changed<Session> changed)
    {
        SceneContext context = changed.Behaviour.Context;
        SessionStates state = changed.Behaviour.SessionState;

        switch (state)
        {
            case SessionStates.WaitingForPlayers:
                context.UI.EnableMenu("WaitingForPlayers");
                return;
            
            case SessionStates.StartingGameplay:
                context.UI.EnableMenu("StartingGameplay");
                return;
            
            case SessionStates.Gameplay:
                context.UI.EnableMenu("Gameplay");
                return;
            
            case SessionStates.EndingGameplay:

                return;
            
            case SessionStates.EndingSession:

                return;
        }
    }
    
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
        if (SessionState == SessionStates.StartingGameplay)
        {
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
            }
        }
        else if (SessionState == SessionStates.Gameplay)
        {
            if(Object.HasStateAuthority)
                GameplayTimer += Runner.DeltaTime;
            
            Context.UI.EnableMenu("Gameplay");

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
