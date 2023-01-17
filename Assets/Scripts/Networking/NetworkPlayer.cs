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
    [SerializeField] private PlayerNetworkMovement movement;
    [SerializeField] private CameraLook cameraLook;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private PlayerAttacker attacker;
    [SerializeField] private PlayerHealth health;
    [SerializeField] private PlayerUI ui;

    public PlayerInteractionHandler Interaction => interaction;
    public PlayerNetworkMovement Movement => movement;
    public CameraLook Camera => cameraLook;
    public PlayerInventory Inventory => inventory;
    public PlayerAttacker Attack => attacker;
    public PlayerHealth Health => health;
    public PlayerUI UI => ui;
    
    [HideInInspector]
    public Player Owner;


    private IInputProccesor[] _inputProcessors;

    
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            Context.Input.RequestCursorLock();
            Context.Camera.Disable();
            FindObjectOfType<GenerateQuadTree>().AssignPlayer(transform);

        }

        _inputProcessors = GetComponents<IInputProccesor>();

        if (Object.HasStateAuthority)
        {
            Movement.CanMove = true;
        }

        SetupPlayer();
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
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput<NetworkInputData>(out var input))
        {
            return;
        }

        for (int i = 0; i < _inputProcessors.Length; i++)
        {
            _inputProcessors[i].ProcessInput(input);
        }
    }


    public void PlayerLeft(PlayerRef player)
    {
        if (player != Object.InputAuthority)
        {
            return;
        }
        
        Runner.Despawn(Object);
    }

    public void OnInventoryToggled(bool state)
    {
        Movement.CanMove = !state;
        Camera.CanLook = !state;

        if (Object.HasInputAuthority)
        {
            if (state)
            {
                Context.Input.RequestCursorRelease();
            }
            else
            {
                Context.Input.RequestCursorLock();
            }
        }
    }
}
