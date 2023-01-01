using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;
using Behaviour = UnityEngine.Behaviour;

public class NetworkPlayer : ContextBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; set; }

    [Networked(OnChanged = nameof(OnNameChanged))]
    public NetworkString<_16> PlayerName { get; set; }
    
    

    [SerializeField] private Behaviour[] remoteComponentsToDisable;
    [SerializeField] private GameObject[] remoteGameObjectsDisable;
    
    [Space]
    
    public UnityEvent onLocalPlayerInit;
    
    
     public PlayerInteractionHandler Interaction { get; private set; } 
     public PlayerMovementHandler Movement { get; private set; }
     public CameraLook Camera { get; private set; } 
     public PlayerInventory Inventory { get; private set; }
     public PlayerAttacker Attack { get; private set; }

     [HideInInspector]
     public Player Owner;

     private bool _requestRespawn;
     
    protected override void Awake()
    {
        base.Awake();
        
        Interaction = GetComponent<PlayerInteractionHandler>();
        Movement = GetComponent<PlayerMovementHandler>();
        Camera = GetComponentInChildren<CameraLook>();
        Inventory = GetComponent<PlayerInventory>();
        Attack = GetComponent<PlayerAttacker>();
    }
    
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            RPC_RequestUpdatePlayerName(Object.HasStateAuthority ? "PLAYER (HOST)" : "PLAYER (CLIENT)");
            Context.Camera.SetActive(false);
        }
        
        SetupPlayer();
    }

    public override void FixedUpdateNetwork()
    {
        if (_requestRespawn)
        {
            RespawnPlayer();
            _requestRespawn = false;
        }
    }

    private void RespawnPlayer()
    {
        if (!Object.HasInputAuthority)
        {
            return;
        }
        
        Camera.CanLook = true;
        Movement.CanMove = true;
        Inventory.CanUse = true;
    }

    public void OnDeath()
    {
        if (!Object.HasInputAuthority)
        {
            return;
        }
        
        Camera.CanLook = false;
        Movement.CanMove = false;
        Inventory.CanUse = false;
    }

    private static void OnNameChanged(Changed<NetworkPlayer> changed)
    {
        changed.Behaviour.gameObject.name = changed.Behaviour.PlayerName.ToString();
    }

    private void SetupPlayer()
    {
        //is this player local
        if (!Object.HasInputAuthority)
        {
            for (int i = 0; i < remoteComponentsToDisable.Length; i++)
            {
                remoteComponentsToDisable[i].enabled = false;
            }

            for (int i = 0; i < remoteGameObjectsDisable.Length; i++)
            {
                remoteGameObjectsDisable[i].SetActive(false);
            }
        }
        else
        {
            onLocalPlayerInit?.Invoke();
        }

        string playerName = Object.HasStateAuthority ? "PLAYER (HOST)" : "PLAYER (CLIENT)";

        if (Object.HasInputAuthority)
        {
            RPC_RequestUpdatePlayerName(playerName);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestUpdatePlayerName(string playerName)
    {
        PlayerName = playerName;
    }


    public void PlayerLeft(PlayerRef player)
    {
        if (player != Object.InputAuthority)
        {
            return;
        }
        
        Runner.Despawn(Object);
    }

    public void RequestRespawn()
    {
        _requestRespawn = true;
    }
}
