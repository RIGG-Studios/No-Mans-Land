using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;


[RequireComponent(typeof(ShipBuoyancy))]
public class Ship : NetworkBehaviour
{
    [Networked(OnChanged = nameof(HasPilotChanged))]
    public bool HasPilot { get; set; }

    [SerializeField] private float speed;
    
    private Rigidbody _rigidbody;

    private Vector3 _velocity;
    private Transform _point;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void FixedUpdateNetwork()
    {
        _rigidbody.AddForce(_velocity, ForceMode.Force);
    }

    private void SetVelocity(Vector3 velocity)
    {
        _velocity = velocity;
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
        HasPilot = false;
    }
    

    private static void HasPilotChanged(Changed<Ship> changed)
    {
        
    }
}
