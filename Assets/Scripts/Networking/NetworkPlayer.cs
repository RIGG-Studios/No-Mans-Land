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
    

    [Header("Network")]
    [SerializeField] private Behaviour[] remoteComponentsToDisable;
    [SerializeField] private GameObject[] remoteGameObjectsDisable;
    
    public UnityEvent onLocalPlayerInit;

    [Header("Local")]
    [SerializeField] private PlayerInteractionHandler interaction;
    [SerializeField] private PlayerMovementHandler movement;
    [SerializeField] private CameraLook cameraLook;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private PlayerAttacker attacker;
    [SerializeField] private PlayerHealth health;
    
    public PlayerInteractionHandler Interaction => interaction;
    public PlayerMovementHandler Movement => movement;
    public CameraLook Camera => cameraLook;
    public PlayerInventory Inventory => inventory;
    public PlayerAttacker Attack => attacker;
    public PlayerHealth Health => health;
    
    [HideInInspector]
    public Player Owner;

    private bool _requestRespawn;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            Context.Camera.SetActive(false);
        }

        gameObject.name = Owner.PlayerName.ToString();
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
