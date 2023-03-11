using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class PlayerNetworkMovement : ContextBehaviour
{
    private PlayerMovement _movementHandler;
    private NetworkPlayer _player;

    [Networked]
    private NetworkButtons ButtonsPrevious { get; set; }
    
    [Networked]
    public NetworkBool IsSprinting { get; set; }
    
    [Networked]
    public NetworkBool IsMoving { get; set; }
    
    [Networked]
    public NetworkBool CameraSubmerged { get; set; }
    
    [Networked]
    public NetworkBool InWater { get; set; }
    
    [Networked]
    public NetworkBool IsFalling { get; set; }
    
    [Networked]
    public NetworkBool CanMove { get; set; }
    
    [Networked]
    public NetworkBool IsGrounded { get; set; }
    
    [Networked]
    public NetworkBool InLadderTrigger { get; set; }
    
    [Networked]
    public NetworkBool IsSwimming { get; set; }

    [Networked]
    public PlayerStates CurrentState { get; set; }

    public PlayerStates RequestedState { get; set; }
    
    [Networked]
    public float Vertical { get; private set; }
    
    [Networked]
    public float Horizontal { get; private set; }

    [Networked] 
    public NetworkBehaviour Cannon { get; set; }
    
    private WaterSearchParameters _searchParameters;
    private WaterSearchResult _searchResult;

    private WaterSurface _waterSurface;

    private float _fallStartPos;
    private float _fallingDist;


    public float maxHeightAboveWater;
    protected override void Awake()
    {
        base.Awake();
        
        _movementHandler = GetComponent<PlayerMovement>();
        _player = GetComponent<NetworkPlayer>();
        _waterSurface = FindFirstObjectByType<WaterSurface>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput<NetworkInputData>(out NetworkInputData input))
        {
            return;
        }

        switch (CurrentState)
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
        CanMove = !_player.Inventory.IsOpen && !_player.Health.IsDead && !_player.Pause.IsOpen;
        
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

                
        _searchParameters.startPositionWS = _searchResult.candidateLocationWS;
        _searchParameters.targetPositionWS = transform.position;
        _searchParameters.error = 0.01f;
        _searchParameters.maxIterations = 8;

        _waterSurface.ProjectPointOnWaterSurface(_searchParameters, out _searchResult);

        IsMoving = input.MovementInput != Vector2.zero;
        InWater = (_player.transform.position.y - 1.0f < _searchResult.projectedPositionWS.y);
        CameraSubmerged = (_player.Camera.transform.position.y < _searchResult.projectedPositionWS.y);
        IsGrounded = CheckForGround();

        float waterHeightDiff = transform.position.y - _searchResult.projectedPositionWS.y;

        IsSwimming = waterHeightDiff < maxHeightAboveWater;
        
        if (InLadderTrigger)
        {
            _movementHandler.MoveLadder(input, Runner.DeltaTime);
        }
        else if (IsSwimming)
        {
            _movementHandler.MoveSwim(input, IsSprinting, Runner.DeltaTime, _searchResult);
        }
        else
        {
            _movementHandler.Move(input, Runner.DeltaTime);
        }

        _movementHandler.UpdateCameraRotation(input);

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
        
        if (pressed.IsSet(PlayerButtons.Jump) && !input.IsAiming && !input.IsReloading && IsGrounded)
        {
            _movementHandler.Jump();
        }

        Vertical = input.MovementInput.y;
        Horizontal = input.MovementInput.x;
        
        if (IsGrounded)
        {
            /*/
            if (IsFalling)
            {
                IsFalling = false;

                if (_fallingDist >= 3.5f && Object.HasStateAuthority)
                {
                    float dmg = Mathf.RoundToInt(_fallingDist * 3.5f);

                    HitData hitData = new HitData() { Damage = dmg };
                    _player.Health.Damage(ref hitData);
                }
                _fallingDist = 0.0f;
            }
            /*/
        }
        else
        {
            if (!IsFalling)
            {
                IsFalling = true;
                _fallStartPos = transform.position.y;
            }
            else
            {
                _fallingDist = _fallStartPos - transform.position.y;
            }
        }
    }

    private int _lastWaveCount;

    private void LateUpdate()
    {
        
    }

    private void ShipMovement(NetworkInputData input)
    {
        _movementHandler.UpdateCameraRotation(input);
    }

    private void CannonMovement(NetworkInputData input)
    {
        
    }

    private bool CheckForGround()
    {
        return Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 2.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Object.HasStateAuthority && other.GetComponent<Ladder>() != null)
        {
            InLadderTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Object.HasStateAuthority && other.GetComponent<Ladder>() != null)
        {
            InLadderTrigger = false;
        }
    }

    public void OnButtonInteract(IInteractable interactable)
    {
        if (interactable.ID == "Chest")
        {
            
        }

        if (interactable.ID == "ShipWheel")
        {
            CurrentState = PlayerStates.ShipController;
            _player.Inventory.HideCurrentItem();
        }

        if (interactable.ID == "Cannon")
        {
            CurrentState = PlayerStates.CannonController;
            _player.Inventory.HideCurrentItem();
            Cannon = interactable as Cannon;
        }
    }

    public void OnButtonStopInteract(IInteractable interactable)
    {
        if (interactable.ID == "ShipWheel")
        {
            CurrentState = PlayerStates.PlayerController;
        }
        
        if (interactable.ID == "Cannon")
        {
            CurrentState = PlayerStates.PlayerController;
        }
    }
}
