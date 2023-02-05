using System;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem.Processors;


[Serializable]
public struct PlayerStats : INetworkStruct
{
    public PlayerRef PlayerRef;
    
    public short Kills;
    public short Deaths;
    public short Score;
    public byte TeamID;
}
public enum StateTypes : byte
{
    Dead,
    Respawning,
    Alive
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
    public static Player Local;
    
    [Networked(OnChanged = nameof(OnActiveNetworkPlayerChanged), OnChangedTargets = OnChangedTargets.All), HideInInspector]
    public NetworkPlayer ActivePlayer { get; private set; }

    [Networked]
    public ref PlayerStats Stats => ref MakeRef<PlayerStats>();
    
    [Networked(OnChanged = nameof(OnNameChanged))]
    public NetworkString<_16> PlayerName { get; set; }
    
    [Networked(OnChanged = nameof(OnStateChanged))]
    public StateTypes State { get; set; }


    
    [SerializeField] private NetworkPlayer playerPrefab;

    public NetworkPlayer PlayerPrefab => playerPrefab;

    protected override void Awake()
    {
        base.Awake();
        
        DontDestroyOnLoad(gameObject);
    }

    public void AssignNetworkPlayer(NetworkPlayer player)
    {
        if (Object.HasInputAuthority)
        {
            Context.UI.CloseAllMenus();
        }

        ActivePlayer = player;
        State = StateTypes.Alive;
        ActivePlayer.Owner = this;
    }
    
    private static void OnNameChanged(Changed<Player> changed)
    {
        changed.Behaviour.gameObject.name = changed.Behaviour.PlayerName.ToString() + "_PERSISTANT";
    }

    private static void OnStateChanged(Changed<Player> changed)
    {
        if (!changed.Behaviour.Object.HasInputAuthority)
        {
            return;
        }

        if (changed.Behaviour.State == StateTypes.Dead)
        {
            changed.Behaviour.Context.Camera.Enable(SceneCamera.CameraTypes.Deploy);
            changed.Behaviour.Context.Input.RequestCursorRelease();
            changed.Behaviour.Context.UI.EnableMenu("SpawnSelectionMenu");
        }
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
        State = StateTypes.Alive;
        
        
        
        if (Object.HasInputAuthority)
        {
            RPC_RequestUpdatePlayerName(ClientInfo.ClientName);
            Local = this;
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

    public void IncrementStat(StatTypes type, int value)
    {
        switch (type)
        {
            case StatTypes.Deaths:
                Stats.Deaths += (short)value;
                return;
            
            case StatTypes.Kills:
                Stats.Kills += (short)value;
                return;
            
            
            case StatTypes.Score:
                Stats.Score += (short)value;
                return;
            
            case StatTypes.TeamID:
                Stats.TeamID += (byte)value;
                return;
        }
    }
    
    public void DecrementStat(StatTypes type, int value)
    {
        switch (type)
        {
            case StatTypes.Deaths:
                Stats.Deaths -= (short)value;
                return;
            
            case StatTypes.Kills:
                Stats.Kills -= (short)value;
                return;

            case StatTypes.Score:
                Stats.Score -= (short)value;
                return;
            
            case StatTypes.TeamID:
                Stats.TeamID -= (byte)value;
                return;
        }
    }
}
