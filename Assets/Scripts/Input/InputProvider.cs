using System;
using Fusion;
using UnityEngine;


public class InputProvider : InputBase
{
    private Vector2 _moveDir;
    private NetworkPlayer _player;

    private bool _isSprintPressed;
    private bool _isJumpPressed;
    private bool _isFirePressed;

    private PlayerStates _currentState;
    private int _currentWeaponID;

    private bool _isAiming;
    private bool _isReloading;

    private bool _slot1Pressed;
    private bool _slot2Pressed;
    private bool _slot3Pressed;
    private bool _slot4Pressed;
    private bool _slot5Pressed;

    private bool _inventoryToggle;

    
    private NetworkInput _networkInput;
    

    private void Start()
    {
        _player = GetComponentInChildren<NetworkPlayer>();


        InputActions.Player.Jump.performed += ctx => _isJumpPressed = true;
        InputActions.Player.Fire.performed += ctx => OnFirePressed();

        InputActions.Player.Slot1.performed += ctx => _slot1Pressed = true;
        InputActions.Player.Slot2.performed += ctx => _slot2Pressed = true;
        InputActions.Player.Slot3.performed += ctx => _slot3Pressed = true;
        InputActions.Player.Slot4.performed += ctx => _slot4Pressed = true;
        InputActions.Player.Slot5.performed += ctx => _slot5Pressed = true;
        InputActions.Player.ToggleInventory.performed += ctx => _inventoryToggle = true;
    }

    private void Update()
    {
        if (NetworkPlayer.Local == null)
        {
            return;
        }
        
        _moveDir = InputActions.Player.Move.ReadValue<Vector2>();
        _isSprintPressed = InputActions.Player.Run.IsPressed();


        _isReloading = InputActions.Player.Reload.IsPressed();
        _isAiming = InputActions.Player.Aim.IsPressed();


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
        if (NetworkPlayer.Local == null)
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
        if (_player == null)
        {
            return;
        }

        if (!Object.HasInputAuthority)
        {
            return;
        }
        
        
        
        NetworkInputData tickInput = new NetworkInputData();

        if (!_player.Inventory.IsOpen)
        {
            tickInput.MovementInput = _moveDir;
        }

        tickInput.LookForward = _player.Camera.PlayerRotation;
        tickInput.LookVertical = _player.Camera.CameraRotation;

        tickInput.RawLookX = _player.Camera.RawLookX;
        tickInput.RawLookY = _player.Camera.RawLookY; 
        tickInput.IsAiming = _isAiming;
        tickInput.IsReloading = _isReloading;
        tickInput.CurrentWeaponID = _currentWeaponID;

        tickInput.Buttons.Set(PlayerButtons.Fire, _isFirePressed);
        tickInput.Buttons.Set(PlayerButtons.Sprint, _isSprintPressed);
        tickInput.Buttons.Set(PlayerButtons.Reload, _isReloading);
        

        tickInput.Buttons.Set(PlayerButtons.Jump, _isJumpPressed);
        tickInput.Buttons.Set(PlayerButtons.Slot1, _slot1Pressed);
        tickInput.Buttons.Set(PlayerButtons.Slot2, _slot2Pressed);
        tickInput.Buttons.Set(PlayerButtons.Slot3, _slot3Pressed);
        tickInput.Buttons.Set(PlayerButtons.Slot4, _slot4Pressed);
        tickInput.Buttons.Set(PlayerButtons.Slot5, _slot5Pressed);
        tickInput.Buttons.Set(PlayerButtons.ToggleInventory, _inventoryToggle);

        
        
        input.Set(tickInput);

        _isFirePressed = false;
        _isJumpPressed = false;
        
        _slot1Pressed = false;
        _slot2Pressed = false;
        _slot3Pressed = false;
        _slot4Pressed = false;
        _slot5Pressed = false;

        _inventoryToggle = false;
    }

    public void ResetSprint()
    {
        _isSprintPressed = false;
    }
}
