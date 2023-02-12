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
    [SerializeField] private Transform wheel;
    
    private Rigidbody _rigidbody;
    private Floater[] _floaters;
    private Ship _ship;

    [Networked] 
    public NetworkBool CanMove { get; set; }
    
    [Networked]
    private float forwardInput { get; set; }

    [Networked]
    private float horizontalInput { get; set; }

    private float _wheelInput;

    private void Awake()
    {
        _ship = GetComponent<Ship>();
        _rigidbody = GetComponent<Rigidbody>();
        _floaters = GetComponentsInChildren<Floater>();
    }

    public override void Spawned()
    {
        CanMove = true;
    }

    public override void FixedUpdateNetwork()
    {
        _rigidbody.AddForce(transform.forward * forwardInput * movementSpeed, ForceMode.Force);

        _rigidbody.AddForceAtPosition(horizontalInput * -_ship.RudderTransform.right * rotationSpeed,
            _ship.RudderTransform.position, ForceMode.Force);
        
        if (!GetInput(out NetworkInputData input) || !CanMove)
        {
            return;
        }
        
        forwardInput = Mathf.Clamp(input.MovementInput.y, 0.0f, maxMovementSpeed);
        horizontalInput = input.MovementInput.x;
    }
    
    public override void Render()
    {
        if (Object.HasInputAuthority)
        {
            _wheelInput += horizontalInput;
            _wheelInput = Mathf.Clamp(_wheelInput, -180, 180f);
            wheel.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, _wheelInput);
        }
    }

    public void AddForce(Vector3 velocity)
    {
        _rigidbody.AddForce(velocity, ForceMode.Force);
    }

    public void ReleaseGravity()
    {
        CanMove = false;

        for (int i = 0; i < 1; i++)
        {
            _floaters[i].gameObject.SetActive(false);
        }
        
        Invoke(nameof(ReleaseAllGravity), 12.0f);
    }

    private void ReleaseAllGravity()
    {
        for (int i = 0; i < _floaters.Length; i++)
        {
            _floaters[i].gameObject.SetActive(false);
        }
    }
}
