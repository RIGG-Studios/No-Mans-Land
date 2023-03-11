using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;


public class Ship : ContextBehaviour
{
    [Networked(OnChanged = nameof(HasPilotChanged))]
    public bool HasPilot { get; set; }
    
    [Networked]
    public byte TeamID { get; set; }
    
    [Networked]
    public int PlayerCount { get; set; }
    
    [Networked]
    public NetworkPlayer Captain { get; set; }

    [SerializeField] private Transform rudderTransform;
    [SerializeField] private ShipSteeringWheelInteraction steeringWheel;
    [SerializeField] private ShipPhysicsHandler physicsHandler;
    [SerializeField] private ShipHealth health;


    public Transform RudderTransform => rudderTransform;
    public ShipPhysicsHandler Physics => physicsHandler;
    public ShipHealth Health => health;
    
    public CannonController[] Cannons { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Cannons = GetComponentsInChildren<CannonController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.IsProxy)
        {
            return;
        }

        if (HasPilot)
        {
            float dist = (steeringWheel.transform.position - Captain.transform.position).magnitude;

            if (dist >= 3.5f)
            {
                Captain.Interaction.TryExitInteract();
                ResetOwnership();
            }
        }
        
        if (Object.HasInputAuthority)
        {
            float min = transform.eulerAngles.y - 90f;
            float max = transform.eulerAngles.y + 90f;
            NetworkPlayer.Local.Camera.UpdateHorizontalLock(min, max, true);
        }
    }

    public void Init(byte id)
    {
        TeamID = id;

        foreach (ISpawnPoint spawnPoint in GetComponentsInChildren<ISpawnPoint>())
        {
            spawnPoint.OverrideTeam(TeamID);
            spawnPoint.Init();
        }
    }

    public void RequestOwnership(PlayerRef playerRef)
    {
        Object.AssignInputAuthority(playerRef);
        
        Context.Gameplay.TryFindPlayer(playerRef, out Player player);

        if (player != null)
        {
            Captain = player.ActivePlayer;
        }
        
        HasPilot = true;
    }

    public void ResetOwnership()
    {
        Object.AssignInputAuthority(default);
        Captain = null;
        HasPilot = false;
    }

    private static void HasPilotChanged(Changed<Ship> changed)
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out NetworkPlayer player))
        {
            PlayerCount++;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out NetworkPlayer player))
        {
            PlayerCount--;
        }
    }
}
