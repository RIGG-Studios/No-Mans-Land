using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerNetworkMovement : ContextBehaviour
{
    private PlayerMovement _movementHandler;

    [Networked]
    private NetworkButtons ButtonsPrevious { get; set; }
    
    [Networked]
    public bool IsSprinting { get; set; }
    
    [Networked]
    public bool IsMoving { get; set; }
    
    [Networked]
    public bool CanMove { get; set; }
    
    [Networked]
    public int CurrentState { get; set; }
    
    public int RequestedState { get; set; }
    
    public float Vertical { get; private set; }
    public float Horizontal { get; private set; }
    
    protected override void Awake()
    {
        base.Awake();
        
        _movementHandler = GetComponent<PlayerMovement>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput<NetworkInputData>(out NetworkInputData input))
        {
            return;
        }

        if (Object.HasStateAuthority)
        {
            CurrentState = input.CurrentState;
        }

        switch (input.CurrentState)
        {
            case 0:
                CharacterMovement(input);
                break;
            
            case 1:
                CannonMovement(input);
                break;
            
            case 2:
                ShipMovement(input);
                break;
        }
    }

    private void CharacterMovement(NetworkInputData input)
    {
        if (!CanMove)
        {
            return;
        }
        
        NetworkButtons pressed = input.Buttons.GetPressed(ButtonsPrevious);
        NetworkButtons released = input.Buttons.GetReleased(ButtonsPrevious);
        
        ButtonsPrevious = input.Buttons;
        
        _movementHandler.Move(input, Runner.DeltaTime);
        _movementHandler.UpdateCameraRotation(input);
        IsMoving = input.MovementInput != Vector2.zero;
        
        if (pressed.IsSet(PlayerButtons.Sprint) && !input.IsAiming && !input.IsReloading)
        {
            IsSprinting = true;
            _movementHandler.ToggleSprint(IsSprinting);
        }

        if (IsSprinting && input.IsAiming)
        {
            IsSprinting = false;
            _movementHandler.ToggleSprint(IsSprinting);
        }
        
        else if (released.IsSet(PlayerButtons.Sprint) || !IsMoving)
        {
            IsSprinting = false;
            _movementHandler.ToggleSprint(IsSprinting);
        }
        
        if (pressed.IsSet(PlayerButtons.Jump) && !input.IsAiming && !input.IsReloading)
        {
            _movementHandler.Jump();
        }

        if (Object.HasInputAuthority)
        {
            Vertical = input.MovementInput.y;
            Horizontal = input.MovementInput.x;
        }
    }

    private void ShipMovement(NetworkInputData input)
    {
        _movementHandler.UpdateCameraRotation(input);
    }

    private void CannonMovement(NetworkInputData input)
    {
        
    }

    public void OnButtonInteract(IInteractable interactable)
    {
        if (interactable.ID == "Chest")
        {
            
        }

        if (interactable.ID == "ShipWheel")
        {
            RequestedState = 2;
        }
    }

    public void OnButtonStopInteract(IInteractable interactable)
    {
        if (interactable.ID == "ShipWheel")
        {
            RequestedState = 0;
        }
    }
}