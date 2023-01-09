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


    public override void FixedUpdateNetwork()
    {
        for (int i = 0; i < _floaters.Length; i++)
        {
          //  _floaters[i].UpdateBuoyancy(Runner.DeltaTime);
        }
        
        
        if (!GetInput(out NetworkInputData input))
        {
            Debug.Log("Not getting inpuit");
            return;
        }
        
        _rigidbody.AddForce(transform.forward * input.MovementInput.y * movementSpeed, ForceMode.Force);
        _rigidbody.AddForceAtPosition(input.MovementInput.x * -_ship.RudderTransform.right * movementSpeed, _ship.RudderTransform.position, ForceMode.Force);
    }
}
