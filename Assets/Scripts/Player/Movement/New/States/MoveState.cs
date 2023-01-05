using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class MoveState : State
{
    private float _gravity;
    private float _movementSpeed;
    private Vector3 _slopeNormal;
    private Rigidbody _rigidbody;
    private Transform _transform;
    private NetworkRunner _runner;

    public override void Init(PlayerMovementHandler movementHandler)
    {
        base.Init(movementHandler);

        _gravity = movementHandler.gravity;
        _movementSpeed = movementHandler.movementSpeed;

        _rigidbody = movementHandler.GetComponent<Rigidbody>();
        _transform = movementHandler.transform;
        _runner = movementHandler.Runner;
    }

    public override void Move(NetworkInputData input)
    {
        UpdatePosition(input);
        UpdateRotation(input);
    }
    
    private void UpdateRotation(NetworkInputData networkInputData)
    {
        _transform.rotation = Quaternion.Slerp(_transform.rotation, networkInputData.LookForward, 1f);
    }
    
    private void UpdatePosition(NetworkInputData networkInputData)
    {
        Vector2 movementInput = networkInputData.MovementInput.normalized;

        _rigidbody.AddForce(Physics.gravity * _gravity, ForceMode.Force);
        _rigidbody.useGravity = true;

        Vector3 forward = _transform.forward;
            
        if (OnSlope())
        {
            forward = Vector3.ProjectOnPlane(forward, _slopeNormal);
        }
            
        Vector3 nextPos = _transform.position +
                          (forward * movementInput.y + _transform.right * movementInput.x) * _movementSpeed *
                          _runner.DeltaTime;
            
        _rigidbody.MovePosition(nextPos);
    }
    
    private bool OnSlope()
    {
        RaycastHit hit;

        if (Physics.Raycast(_transform.position, Vector3.down, out hit, 2 * 0.5f + 0.5f))
        {
            if (hit.normal != Vector3.up)
            {
                _slopeNormal = hit.normal;
                return true;
            }
        }

        return false;
    }
}
