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
    public PlayerStates CurrentState { get; set; }
    
    public PlayerStates RequestedState { get; set; }
    
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
            case PlayerStates.PlayerController:
                CharacterMovement(input);
                break;
            
            case PlayerStates.CannonController:
                CannonMovement(input);
                break;
            
            case PlayerStates.ShipController:
                ShipMovement(input);
                break;
        }
    }

    private void CharacterMovement(NetworkInputData input)
    {
        if (!CanMove)
        {
            IsMoving = false;
            IsSprinting = false;
            _movementHandler.ToggleSprint(IsSprinting);
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
            RequestedState = PlayerStates.ShipController;
            NetworkPlayer.Local.Inventory.HideCurrentItem();
        }

        if (interactable.ID == "Cannon")
        {
            RequestedState = PlayerStates.CannonController;
            NetworkPlayer.Local.Inventory.HideCurrentItem();
        }
    }

    public void OnButtonStopInteract(IInteractable interactable)
    {
        if (interactable.ID == "ShipWheel")
        {
            RequestedState = PlayerStates.PlayerController;
        }
        
        if (interactable.ID == "Cannon")
        {
            RequestedState = PlayerStates.PlayerController;
        }
    }
}
