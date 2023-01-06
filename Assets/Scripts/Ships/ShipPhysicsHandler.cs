using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ShipPhysicsHandler : NetworkBehaviour
{
    [SerializeField] private float movementSpeed;
    
    private Rigidbody _rigidbody;
    private Floater[] _floaters;

    private Ship _ship;


    private void Awake()
    {
        _floaters = GetComponentsInChildren<Floater>();
        _ship = GetComponent<Ship>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void MoveShip(ShipMovementData input)
    {
        Vector3 velocity = input.ForwardVelocity;
        
        _rigidbody.AddForce(velocity, ForceMode.Force);
        
        _rigidbody.AddForceAtPosition(input.RotationalVelocity, _ship.RudderTransform.position, ForceMode.Force);
    }
}
