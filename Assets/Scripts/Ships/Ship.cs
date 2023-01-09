using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;


public class Ship : NetworkBehaviour
{
    [Networked(OnChanged = nameof(HasPilotChanged))]
    public bool HasPilot { get; set; }

    [SerializeField] private Transform rudderTransform;

    public Transform RudderTransform => rudderTransform;
    
    private ShipPhysicsHandler _shipPhysics;

    private void Awake()
    {
        _shipPhysics = GetComponent<ShipPhysicsHandler>();
    }
    

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestPilotChange(PlayerRef playerRef)
    {
        Object.AssignInputAuthority(playerRef);
        HasPilot = true;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestResetPilot()
    {
        Object.AssignInputAuthority(default);
        HasPilot = false;
    }
    

    private static void HasPilotChanged(Changed<Ship> changed)
    {
        
    }
}
