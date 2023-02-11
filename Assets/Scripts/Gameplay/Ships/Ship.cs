using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;


public class Ship : NetworkBehaviour
{
    [Networked(OnChanged = nameof(HasPilotChanged))]
    public bool HasPilot { get; set; }
    
    [Networked]
    public byte TeamID { get; set; }

    [SerializeField] private Transform rudderTransform;
    [SerializeField] private ShipPhysicsHandler physicsHandler;
    [SerializeField] private ShipHealth health;
    
    
    public Transform RudderTransform => rudderTransform;
    public ShipPhysicsHandler Physics => physicsHandler;
    public ShipHealth Health => health;
    
    public CannonController[] Cannons { get; private set; }

    private void Awake()
    {
        Cannons = GetComponentsInChildren<CannonController>();
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
        HasPilot = true;
    }

    public void ResetOwnership()
    {
        Object.AssignInputAuthority(default);
        HasPilot = false;
    }

    private static void HasPilotChanged(Changed<Ship> changed)
    {
        
    }
}
