using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ShipPhysicsHandler : NetworkBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float maxMovementSpeed;
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

        float verticalInput = Mathf.Clamp(input.MovementInput.y, 0.0f, maxMovementSpeed);
        
        _rigidbody.AddForce(transform.forward * verticalInput * movementSpeed, ForceMode.Force);

        if (verticalInput > 0)
        {
            _rigidbody.AddForceAtPosition(input.MovementInput.x * -_ship.RudderTransform.right * rotationSpeed,
                _ship.RudderTransform.position, ForceMode.Force);
        }
        
       // Debug.Log(_rigidbody.velocity);
    }
}
