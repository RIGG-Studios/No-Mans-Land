using System;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public enum PlayerMovementStates
{
    Idle,
    Moving,
    Sprinting,
    InAir,
    Climbing
}

public class PlayerMovementHandler : NetworkBehaviour
{
    public bool CanMove { get; set; }
    public bool IsMovingForward { get; private set; }

    public PlayerMovementStates MovementState { get; private set; }

    
    public float movementSpeed = 7;
    public float defaultFOV = 90f;
    public float jumpSpeed = 10f;
    public float sprintSpeed = 10f;
    public float sprintFOV = 110f;
    public float gravity = 10f;
    public float climbSpeed = 5f;
    public float groundCheckDist = 0.4f;

    
    private bool _canSendInputToStates = true;
    private Rigidbody _rigidbody;
    private CameraLook _cameraLook;
    private PlayerMovementInputHandler _inputHandler;
    
    private float _vertical;
    private bool _isGrounded;
    private float _horizontal;

    private Ladder _ladder;
    private bool _onLadder;

    private Vector3 _slopeNormal;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _cameraLook = GetComponentInChildren<CameraLook>();
        _inputHandler = GetComponent<PlayerMovementInputHandler>();
    }

    private void Start()
    {
        CanMove = true;
    }

    private void Update()
    {
        if (!Object.HasInputAuthority)
        {
            return;
        }
        
        IsMovingForward = _vertical >= 0.5f;
        
        var fov = MovementState == PlayerMovementStates.Sprinting ? sprintFOV : defaultFOV;
        
        _cameraLook.SetFOV(fov);


        if (MovementState == PlayerMovementStates.Sprinting && !IsMovingForward)
        {
            _inputHandler.DisableSprint();
            MovementState = PlayerMovementStates.Moving;
        }
        
        CheckForGround();
    }
    
    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData networkInputData))
        {
            return;
        }

        if (!CanMove)
        {
            return;
        }
        
        UpdateRotation(networkInputData);
        UpdatePosition(networkInputData);
        UpdateStates(networkInputData);
            
        if (networkInputData.IsJumpPressed && _isGrounded)
        {
            _rigidbody.velocity /= 2;
            _rigidbody.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }
    }

    private void UpdateRotation(NetworkInputData networkInputData)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, networkInputData.LookForward, 1f);
    }

    private void UpdatePosition(NetworkInputData networkInputData)
    {
        Vector2 movementInput = networkInputData.MovementInput.normalized;

        _horizontal = movementInput.x;
        _vertical = movementInput.y;

        if (_onLadder)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.useGravity = false;
            Vector3 forward = Vector3.up;

            Vector3 nextPos = transform.position +
                              (forward * _vertical + transform.right * _horizontal) * CalculateSpeed() *
                              Runner.DeltaTime;
            
            _rigidbody.MovePosition(nextPos);
        }
        else
        {
            _rigidbody.AddForce(Physics.gravity * gravity, ForceMode.Force);

            _rigidbody.useGravity = true;

            Vector3 forward = transform.forward;
            
            if (OnSlope())
            {
                forward = Vector3.ProjectOnPlane(forward, _slopeNormal);
            }
            
            Vector3 nextPos = transform.position +
                              (forward * _vertical + transform.right * _horizontal) * CalculateSpeed() *
                              Runner.DeltaTime;
            
            _rigidbody.MovePosition(nextPos);

        }
    }

    private void UpdateStates(NetworkInputData networkInputData)
    {
        if (_vertical == 0.0f && _horizontal == 0.0f)
        {
            MovementState = PlayerMovementStates.Idle;
        }
        else
        {
            MovementState = PlayerMovementStates.Moving;
        }

        if (networkInputData.IsSprintPressed)
        {
            MovementState = PlayerMovementStates.Sprinting;
        }

        if (_onLadder)
        {
            MovementState = PlayerMovementStates.Climbing;
        }
    }

    private float CalculateSpeed()
    {
        var speed = 0.0f;

        switch (MovementState)
        {
            case PlayerMovementStates.Idle:
                speed = movementSpeed;
                break;
                
            case PlayerMovementStates.Moving:
                speed = movementSpeed;
                break;
                
            case PlayerMovementStates.Sprinting:
                speed = sprintSpeed;
                break;
            
            case PlayerMovementStates.Climbing:
                speed = climbSpeed;
                break;
        }
        
        
        return speed;
    }

    private void CheckForGround()
    {
        RaycastHit hit;
        _isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 1, 0), groundCheckDist);
    }

    private bool OnSlope()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2 * 0.5f + 0.5f))
        {
            if (hit.normal != Vector3.up)
            {
                _slopeNormal = hit.normal;
                return true;
            }
        }

        return false;
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
        if (interactable.ID != "Chest")
        {
            return;
        }

        NetworkPlayer.Local.Inventory.ToggleInventory();
        _cameraLook.CanLook = false;
        CanMove = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    public void OnStopButtonInteract(IInteractable interactable)
    {
        if (interactable.ID != "Chest")
        {
            return;
        }

        NetworkPlayer.Local.Inventory.ToggleInventory();
        _cameraLook.CanLook = true;
        CanMove = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnInventoryToggled(bool isOpen)
    {
        _cameraLook.CanLook = !isOpen;
        CanMove = !isOpen;
        Cursor.visible = isOpen;
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
