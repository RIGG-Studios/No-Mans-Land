using System;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovementHandler : StateMachine
{
    public bool CanMove { get; set; }
    
    public bool IsMovingForward { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool IsMoving { get; private set; }
    public float Vertical { get; private set; }
    public float Horizontal { get; private set; }

    public float movementSpeed = 7;
    public float jumpSpeed = 10f;
    public float sprintSpeed = 10f;
    public float sprintFOV = 110f;
    public float gravity = 10f;
    public float climbSpeed = 5f;
    public float groundCheckDist = 0.4f;

    
    private bool _canSendInputToStates = true;
    private Rigidbody _rigidbody;
    
    [HideInInspector]
    public CameraLook cameraLook;
    
    public Ship Ship { get; set; }
    
    
    [Networked]
    public NetworkButtons ButtonsPrevious { get; set; }
    
    private Ladder _ladder;
    private bool _onLadder;

    private InputProvider _inputProvider;
    private Vector3 _slopeNormal;

    protected override void Awake()
    {
        base.Awake();
        
        _rigidbody = GetComponent<Rigidbody>();
        cameraLook = GetComponentInChildren<CameraLook>();
        _inputProvider = GetComponent<InputProvider>();
    }

    private void Start()
    {
        InitStates(this);
        EnterState(State.StateTypes.Moving);
        
        CanMove = true;
    }
    
    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData input))
        {
            return;
        }
        
        if (CurrentState != null)
        {
            CurrentState.Move(input);
        }

        if (!CanMove)
        {
            return;
        }
        
        NetworkButtons pressed = input.Buttons.GetPressed(ButtonsPrevious);
        NetworkButtons released = input.Buttons.GetReleased(ButtonsPrevious);

        ButtonsPrevious = input.Buttons;
        
        if (pressed.IsSet(PlayerButtons.Jump))
        {
            EnterState(State.StateTypes.Jumping);
        }
        if (pressed.IsSet(PlayerButtons.Sprint))
        {
            EnterState(State.StateTypes.Sprinting);
        }
        else if ((released.IsSet(PlayerButtons.Sprint) || input.MovementInput == Vector2.zero) && CurrentStateType != State.StateTypes.Sailing)
        {
            EnterState(State.StateTypes.Moving);
            _inputProvider.ResetSprint();
        }

        IsMoving = input.MovementInput != Vector2.zero;
        
        CheckForGround();
        Vertical = input.MovementInput.y;
        Horizontal = input.MovementInput.x;
    }

    public void AddForce(Vector3 velocity, ForceMode forceMode)
    {
        _rigidbody.AddForce(velocity, forceMode);
    }
    

    private void CheckForGround()
    {
        IsGrounded = Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, groundCheckDist);
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Ladder ladder))
        {
            return;
        }

        _ladder = ladder;
        _onLadder = true;
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out Ladder ladder))
        {
            return;
        }

        _ladder = null;
        _onLadder = false;
    }

    public void OnButtonInteract(IInteractable interactable)
    {
        if (interactable.ID == "Cannon")
        {
            if (HasInputAuthority && NetworkPlayer.Local.Inventory.IsOpen)
            {
                NetworkPlayer.Local.Inventory.ToggleInventory();
            }
            
            cameraLook.gameObject.SetActive(false);
            CanMove = false;
        }

        if (interactable.ID == "SteeringWheel")
        {
            ShipSteeringWheelInteraction wheel = interactable as ShipSteeringWheelInteraction;

            if (wheel != null)
            {
                Ship = wheel.Ship;
                
                Debug.Log("Enter sail state");
                EnterState(State.StateTypes.Sailing);
            }
        }
        
        if (interactable.ID != "Chest")
        {
            return;
        }

        NetworkPlayer.Local.Inventory.ToggleInventory();
        cameraLook.CanLook = false;
        CanMove = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    public void OnStopButtonInteract(IInteractable interactable)
    {
        if (interactable.ID == "SteeringWheel")
        {
            EnterState(State.StateTypes.Moving);
        }
        
        if (interactable.ID == "Cannon")
        {
            if (HasInputAuthority && NetworkPlayer.Local.Inventory.IsOpen)
            {
                NetworkPlayer.Local.Inventory.ToggleInventory();
            }
            
            cameraLook.gameObject.SetActive(true);
            CanMove = true;
        }
        
        if (interactable.ID != "Chest")
        {
            return;
        }

        NetworkPlayer.Local.Inventory.ToggleInventory();
        cameraLook.CanLook = true;
        CanMove = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnInventoryToggled(bool isOpen)
    {
        cameraLook.CanLook = !isOpen;
        CanMove = !isOpen;
        Cursor.visible = isOpen;
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
