using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : MoveState
{
    private float _jumpHeight;
    private Rigidbody _rigidbody;

    private bool _previouslyGrounded;


    public override void Init(PlayerMovementHandler movement, StateTypes type)
    {
        base.Init(movement, type);

        _jumpHeight = movement.jumpSpeed;
        _rigidbody = movement.GetComponent<Rigidbody>();
    }

    public override void Enter()
    {
        _rigidbody.AddForce(_rigidbody.transform.up * _jumpHeight, ForceMode.Impulse);
    }

    public override void Move(NetworkInputData input)
    {
        base.Move(input);
        if (!_previouslyGrounded && MovementHandler.IsGrounded)
        {
            Land();
        }
        
        _previouslyGrounded = MovementHandler.IsGrounded;
    }

    private void Land()
    {
        MovementHandler.EnterState(StateTypes.Moving);
    }
}
