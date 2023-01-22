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
    [SerializeField] private Transform targetCameraTransform;
    [SerializeField] private Transform cameraTransform;

    public Transform RudderTransform => rudderTransform;
    
    public void Init(byte id)
    {
        TeamID = id;

        foreach (ISpawnPoint spawnPoint in GetComponentsInChildren<ISpawnPoint>())
        {
            spawnPoint.OverrideTeam(TeamID);
            spawnPoint.Init();
        }
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
