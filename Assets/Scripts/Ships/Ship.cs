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
    
    private ShipPhysicsHandler _shipPhysics;

    private void Awake()
    {
        _shipPhysics = GetComponent<ShipPhysicsHandler>();
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
    
    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData input))
        {
            return;
        }

        if (Object.HasInputAuthority)
        {
            cameraTransform.gameObject.SetActive(true);
            NetworkPlayer.Local.Camera.gameObject.SetActive(false);
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

    public void ResetLocal()
    {
        if (Object.HasInputAuthority)
        {
            cameraTransform.gameObject.SetActive(false);
            NetworkPlayer.Local.Camera.gameObject.SetActive(true);
        }
    }
}
