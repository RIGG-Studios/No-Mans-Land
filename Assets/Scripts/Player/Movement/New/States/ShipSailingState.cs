using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ShipMovementData
{
    public Vector3 ForwardVelocity;

    public Vector3 RotationalVelocity;
}

public class ShipSailingState : State
{
    private Ship _ship;

    private float _forwardSpeed;
    private float _rotationSpeed;

    private Transform _transform;

    public override void Init(PlayerMovementHandler movementHandler, StateTypes type)
    {
        base.Init(movementHandler, type);

        _transform = movementHandler.transform;
    }
    
    public override void Enter()
    {
        _ship = MovementHandler.Ship;
        _forwardSpeed = 400f;
        _rotationSpeed = 200f;
        MovementHandler.CanMove = false;
    }

    public override void Exit()
    {
        MovementHandler.CanMove = true;
    }
    
    public override void Move(NetworkInputData input)
    {
        //update the player rotation based on mouse look
        UpdateRotation(input);
        
        Vector3 forwardVelocity = _ship.transform.forward * input.MovementInput.y * _forwardSpeed;
        Vector3 rotationalVelocity = -_ship.RudderTransform.right * input.MovementInput.x * _rotationSpeed;
        
        ShipMovementData shipMovementData = new ShipMovementData()
        {
            ForwardVelocity = forwardVelocity,
            RotationalVelocity = rotationalVelocity
        };
        
      //  _ship.MoveShip(shipMovementData);
    }

    private void UpdateRotation(NetworkInputData networkInputData)
    {
        _transform.rotation = Quaternion.Slerp(_transform.rotation, networkInputData.LookForward, 1f);
    }
}
