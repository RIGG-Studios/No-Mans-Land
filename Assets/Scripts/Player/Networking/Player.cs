using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public struct PlayerStats : INetworkStruct
{
    public PlayerRef PlayerRef;
    
    public short Kills;
    public short Deaths;
    public short Score;
    public byte TeamID;
}

public enum StatTypes : byte
{
    Kills,
    Deaths,
    Score,
    TeamID
}

public class Player : ContextBehaviour
{
    [Networked(OnChanged = nameof(OnActiveNetworkPlayerChanged), OnChangedTargets = OnChangedTargets.All), HideInInspector]
    public NetworkPlayer ActivePlayer { get; private set; }

    [Networked]
    public ref PlayerStats Stats => ref MakeRef<PlayerStats>();
    
    [Networked(OnChanged = nameof(OnNameChanged))]
    public NetworkString<_16> PlayerName { get; set; }


    
    [SerializeField] private NetworkPlayer playerPrefab;

    public NetworkPlayer PlayerPrefab => playerPrefab;

    protected override void Awake()
    {
        base.Awake();
        
        if (Object.HasInputAuthority)
        {
            RPC_RequestUpdatePlayerName(Object.HasStateAuthority ? "PLAYER (HOST)" : "PLAYER (CLIENT)");
        }
    }
    
    public void AssignNetworkPlayer(NetworkPlayer player)
    {
        ActivePlayer = player;
        ActivePlayer.Owner = this;
    }
    
    private static void OnNameChanged(Changed<Player> changed)
    {
        changed.Behaviour.gameObject.name = changed.Behaviour.PlayerName.ToString() + "_PERSISTANT";
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestUpdatePlayerName(string playerName)
    {
        PlayerName = playerName;
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

    public void SetStat(StatTypes type, int value)
    {
        switch (type)
        {
            case StatTypes.Deaths:
                Stats.Deaths = (short)value;
                return;
            
            case StatTypes.Kills:
                Stats.Kills = (short)value;
                return;
            
            
            case StatTypes.Score:
                Stats.Score = (short)value;
                return;
            
            case StatTypes.TeamID:
                Stats.TeamID = (byte)value;
                return;
        }
    }
}
