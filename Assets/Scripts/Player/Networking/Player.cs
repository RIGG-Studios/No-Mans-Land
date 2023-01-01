using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEditor;
using UnityEngine;

public struct PlayerStats : INetworkStruct
{
    public PlayerRef PlayerRef;

    public short Kills;
    public short Deaths;
    public short Score;
    public TickTimer RespawnTimer;
    public byte Position;
}

public class Player : ContextBehaviour
{
    [Networked(OnChanged = nameof(OnActiveNetworkPlayerChanged), OnChangedTargets = OnChangedTargets.All), HideInInspector]
    public NetworkPlayer ActivePlayer { get; private set; }

    [Networked]
    public ref PlayerStats Stats => ref MakeRef<PlayerStats>();
    
    [SerializeField] private NetworkPlayer playerPrefab;

    public NetworkPlayer PlayerPrefab => playerPrefab;
    
    public void AssignNetworkPlayer(NetworkPlayer player)
    {
        ActivePlayer = player;
        ActivePlayer.Owner = this;
    }

    public void ClearNetworkPlayer()
    {
        if (ActivePlayer == null)
        {
            return;
        }

        ActivePlayer.Owner = null;
        ActivePlayer = null;
    }

    public override void Spawned()
    {
        Stats.PlayerRef = Object.InputAuthority;

        if (Context.Gameplay != null)
        {
            Context.Gameplay.Join(this);
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (!hasState)
        {
            return;
        }
        
        if (Context.Gameplay != null)
        {
            Context.Gameplay.Leave(this);
        }

        if (Object.HasStateAuthority && ActivePlayer != null)
        {
            runner.Despawn(ActivePlayer.Object);
        }

        ActivePlayer = null;
    }
    

    private static void OnActiveNetworkPlayerChanged(Changed<Player> changed)
    {
        if (changed.Behaviour.ActivePlayer != null)
        {
            changed.Behaviour.AssignNetworkPlayer(changed.Behaviour.ActivePlayer);
        }
        else
        {
            changed.Behaviour.ClearNetworkPlayer();
        }
    }

}
