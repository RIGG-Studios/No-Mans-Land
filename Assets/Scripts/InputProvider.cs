using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class InputProvider : InputBase
{
    private Vector2 _moveDir;
    private CameraLook _cameraLook;

    private bool _isSprintPressed;
    private bool _isJumpPressed;
    private bool _isFirePressed;


    private NetworkInput _networkInput;


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

        InputActions.Player.Fire.performed += ctx =>
        {
            OnFirePressed();
        };
    }

    private void Update()
    {
        _moveDir = InputActions.Player.Move.ReadValue<Vector2>();
        
        
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
    
    private void OnFirePressed()
    {
        if (NetworkPlayer.Local.Inventory.EquippedItem == null)
        {
            return;
        }

        if (NetworkPlayer.Local.Inventory.IsOpen)
        {
            return;
        }
        
        _isFirePressed = true;
    }
    
    private void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (_cameraLook == null)
        {
            return;
        }

        NetworkInputData tickInput = new NetworkInputData();

        tickInput.MovementInput = _moveDir;
        tickInput.LookForward = _cameraLook.PlayerRotation;
        
        tickInput.Buttons.Set(Buttons.Fire, _isFirePressed);
        tickInput.Buttons.Set(Buttons.Sprint, _isSprintPressed);
        tickInput.Buttons.Set(Buttons.Jump, _isJumpPressed);
        
        input.Set(tickInput);

        _isFirePressed = false;
        _isJumpPressed = false;
    }

    public void ResetSprint()
    {
        _isSprintPressed = false;
    }
}
