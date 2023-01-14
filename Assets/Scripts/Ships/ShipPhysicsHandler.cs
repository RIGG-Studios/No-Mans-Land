using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ShipPhysicsHandler : NetworkBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    
    private Rigidbody _rigidbody;
    private Ship _ship;


    private void Awake()
    {
        _ship = GetComponent<Ship>();
        _rigidbody = GetComponent<Rigidbody>();
    }


    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData input))
        {
            return;
        }
        
        _rigidbody.AddForce(transform.forward * input.MovementInput.y * movementSpeed, ForceMode.Force);
        _rigidbody.AddForceAtPosition(input.MovementInput.x * -_ship.RudderTransform.right * rotationSpeed, _ship.RudderTransform.position, ForceMode.Force);
    }
}
