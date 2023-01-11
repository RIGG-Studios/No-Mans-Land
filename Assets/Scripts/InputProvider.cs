using Fusion;
using UnityEngine;


//network input class, used to send input/data across the network in an secure way
//since Fusion's input system is the main way to drive state in a game, we will also send
//state changes to be executed on the server (State Authority)
public class InputProvider : InputBase
{
    private Vector2 _moveDir;
    private CameraLook _cameraLook;

    private bool _isSprintPressed;
    private bool _isJumpPressed;
    private bool _isFirePressed;

    private int _currentState;
    private int _currentWeaponID;

    private bool _isAiming;
    private bool _isReloading;


    private NetworkInput _networkInput;
    

    private void Start()
    {
        _cameraLook = GetComponentInChildren<CameraLook>();
        

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
        _isSprintPressed = InputActions.Player.Run.IsPressed();
        
        ItemControllerState frameState = NetworkPlayer.Local.Inventory.GetEquippedItemState();
        
        _isReloading = frameState.IsReloading;
        _isAiming = frameState.IsAiming;

        _currentState = NetworkPlayer.Local.Movement.RequestedState;
        _currentWeaponID = NetworkPlayer.Local.Inventory.RequestedEquippedItem;
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

        if (!Object.HasInputAuthority)
        {
            return;
        }
        
        NetworkInputData tickInput = new NetworkInputData();

        tickInput.MovementInput = _moveDir;
        tickInput.LookForward = _cameraLook.PlayerRotation;
        tickInput.IsAiming = _isAiming;
        tickInput.IsReloading = _isReloading;
        tickInput.CurrentState = _currentState;
        tickInput.CurrentWeaponID = _currentWeaponID;
        
        tickInput.Buttons.Set(PlayerButtons.Fire, _isFirePressed);
        tickInput.Buttons.Set(PlayerButtons.Sprint, _isSprintPressed);
        tickInput.Buttons.Set(PlayerButtons.Jump, _isJumpPressed);
        
        input.Set(tickInput);

        _isFirePressed = false;
        _isJumpPressed = false;
    }

    public void ResetSprint()
    {
        _isSprintPressed = false;
    }
}
