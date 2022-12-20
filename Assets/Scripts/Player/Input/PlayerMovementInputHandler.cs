using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerMovementInputHandler : InputBase
{
    private Vector2 _movementDirection;
    private CameraLook _cameraLook;

    private bool _isSprintPressed;
    private bool _isJumpPressed;
    
    private void Start()
    {
        _cameraLook = GetComponentInChildren<CameraLook>();

        InputActions.Player.Run.performed += ctx =>
        {
            _isSprintPressed = !_isSprintPressed;
        };

        InputActions.Player.Jump.performed += ctx =>
        {
            _isJumpPressed = true;
        };
    }
    
    public override void OnEnable()
    {
        base.OnEnable();
        NetworkCallBackEvents.onInput += OnInput;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        NetworkCallBackEvents.onInput -= OnInput;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        NetworkCallBackEvents.onInput -= OnInput;
    }

    private void Update()
    {
        _movementDirection = InputActions.Player.Move.ReadValue<Vector2>();
    }
    
    private void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (_cameraLook == null)
        {
            return;
        }
        
        input.Set(GetNetworkInput());
    }
    
    public NetworkInputData GetNetworkInput()
    {
        var networkInputData = new NetworkInputData()
        {
            MovementInput = _movementDirection,
            LookForward = _cameraLook.PlayerRotation,
            IsJumpPressed = _isJumpPressed,
            IsSprintPressed = _isSprintPressed
        };

        _isJumpPressed = false;

        return networkInputData;
    }

    public void DisableSprint()
    {
        _isSprintPressed = false;
    }
}
