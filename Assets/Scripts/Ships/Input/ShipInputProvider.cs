using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ShipInputProvider : InputBase
{
    private Vector2 _moveInput;

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

    private void Update()
    {
        _moveInput = InputActions.Player.Move.ReadValue<Vector2>();
    }
    
    private void OnInput(NetworkRunner runner, NetworkInput input)
    {
        NetworkShipInputData tickInput = new NetworkShipInputData();

        tickInput.MovementInput = _moveInput;
        
        input.Set(tickInput);
    }
    
    
}
