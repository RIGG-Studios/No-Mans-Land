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
    [SerializeField] private PlayerHealth health;
    [SerializeField] private Character character;
    [SerializeField] private PlayerUI ui;
    [SerializeField] private Crosshair hitMarker;
    [SerializeField] private PlayerPauseManager pause;

    public PlayerInteractionHandler Interaction => interaction;
    public PlayerNetworkMovement Movement => movement;
    public CameraLook Camera => cameraLook;
    public PlayerInventory Inventory => inventory;
    public PlayerHealth Health => health;
    public PlayerUI UI => ui;
    public PlayerPauseManager Pause => pause;
    public Character Character => character;

    public Crosshair HitMarker => hitMarker;

    [HideInInspector]
    public Player Owner;


    private IInputProccesor[] _inputProcessors;

    private Objective _currentObjective;

    
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            Context.Input.RequestCursorLock();
            Context.Camera.Disable(SceneCamera.CameraTypes.Deploy);
            Context.Camera.Disable(SceneCamera.CameraTypes.Scene);
            Context.UI.EnableMenu("Gameplay");
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
        if (_currentObjective != null)
        {
            _currentObjective.OnPlayerLeft(Owner.Stats.TeamID);
            _currentObjective = null;
        }

        if (Object.HasInputAuthority)
        {
            Context.UI.DisableMenu("ObjectiveCapture");
        }
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

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Objective objective))
        {
            return;
        }

        if (Object.HasInputAuthority)
        {
            Context.UI.GetService<ObjectiveCapturingUI>().SetObjective(objective);
            Context.UI.EnableMenu("ObjectiveCapture", false);
        }
            
        if (Object.HasStateAuthority)
        {
            objective.OnPlayerEntered(Owner.Stats.TeamID);
            _currentObjective = objective;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out Objective objective))
        {
            return;
        }
        
        if (Object.HasInputAuthority)
        {
            Context.UI.GetService<ObjectiveCapturingUI>().SetObjective(null);
            Context.UI.DisableMenu("ObjectiveCapture");
        }

        if (Object.HasStateAuthority)
        {
            objective.OnPlayerLeft(Owner.Stats.TeamID);
            _currentObjective = null;
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
}
