
using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


[RequireComponent(typeof(PlayerInteractionInputHandler))]
public class PlayerInteractionHandler : NetworkBehaviour
{
    public bool CanInteract { get; private set; }
    
    [SerializeField, Range(0, 25)] private float maxInteractionDistance;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private Transform lookFromPoint;
    [SerializeField] private GameObject interactUI;
    [SerializeField] private Text interactText;
    
    
    public UnityEvent<IInteractable> onLookAtInteract = new();
    public UnityEvent<IInteractable> onButtonInteract = new();
    public UnityEvent<IInteractable> onButtonInteractStop = new();

    
    [Networked]
    private bool _sentLookAtEvent { get; set; }
    
    [Networked]
    private bool _isInteracting { get; set; }
    
    [Networked]
    private NetworkBehaviour currentInteractable { get; set; }

    private IInteractable _currentInteractable => currentInteractable as IInteractable;

    private NetworkPlayer _networkPlayer;

    private void Awake()
    {
        _networkPlayer = GetComponent<NetworkPlayer>();
    }

    private void Start()
    {
        CanInteract = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData input))
        {
            return;
        }

        if (!_isInteracting)
        {
            ShootRaycast(input);
        }

        bool interactPressed = input.Buttons.IsSet(PlayerButtons.Interact);

        if (_currentInteractable != null && input.Buttons.IsSet(_currentInteractable.ExitKey) && _isInteracting)
        {
            TryExitInteract();
        }
        else if (_currentInteractable != null && interactPressed && !_isInteracting)
        {
            TryButtonInteract();
        }
    }

    private void ShootRaycast(NetworkInputData input)
    {
        Vector3 pos = lookFromPoint.position;
        Vector3 dir = (input.LookForward * input.LookVertical) * Vector3.forward;

        Runner.LagCompensation.Raycast(pos, dir, maxInteractionDistance, Object.InputAuthority, out var hitInfo,
            interactLayer, HitOptions.IncludePhysX);

        if (hitInfo.GameObject == null)
        {
            if (_currentInteractable != null)
            {
                if (Object.HasInputAuthority)
                {
                    _currentInteractable.StopLookAtInteract();
                    interactText.text = "";
                    interactUI.SetActive(false);
                }
                
                _sentLookAtEvent = false;

            }

            return;
        }

        if (hitInfo.GameObject != null && hitInfo.GameObject.TryGetComponent(out IInteractable interactable))
        {
            currentInteractable = interactable as NetworkBehaviour;
            
            if (!_sentLookAtEvent)
            {
                if (Object.HasInputAuthority)
                {
                    onLookAtInteract?.Invoke(_currentInteractable);
                    interactText.text = _currentInteractable.LookAtID;
                    interactUI.SetActive(true);
                }

                _sentLookAtEvent = true;
            }
        }
        else
        {
            if (_currentInteractable != null)
            {
                _sentLookAtEvent = false;

                if (Object.HasInputAuthority)
                {
                    _currentInteractable.StopLookAtInteract();
                    interactText.text = "";
                    interactUI.SetActive(false);
                }
            }
        }
    }


    public void TryButtonInteract()
    {
        if (_currentInteractable == null)
        {
            return;
        }
        
        bool success = _currentInteractable.ButtonInteract(_networkPlayer, out ButtonInteractionData interactionData);

        _isInteracting = success;
        
        if (success)
        {
            if (Object.HasInputAuthority)
            {
                interactUI.SetActive(false);
            }
            
            onButtonInteract?.Invoke(_currentInteractable);


            if (interactionData.StopMovement)
            {
                _networkPlayer.Movement.CanMove = false;
            }

            if (interactionData.EnableMovement)
            {
                _networkPlayer.Movement.CanMove = true;
            }

            if (interactionData.OpenInventory)
            {
                Debug.Log(Object.HasStateAuthority);
                if (!_networkPlayer.Inventory.IsOpen)
                {
                    _networkPlayer.Inventory.ToggleInventory();
                }
            }
            
            if (interactionData.HideInventory)
            {
                if (_networkPlayer.Inventory.IsOpen)
                {
                    _networkPlayer.Inventory.ToggleInventory();
                }
            }

            if (Object.HasInputAuthority)
            {
                if (interactionData.EnableCameraLook)
                {
                    _networkPlayer.Camera.enabled = true;
                }

                if (interactionData.StopCameraLook)
                {
                    _networkPlayer.Camera.enabled = false;
                }
            }
        }
    }

    public void TryExitInteract()
    {
        if (_currentInteractable == null)
        {
            return;
        }
        
        _currentInteractable.StopLookAtInteract();
        onButtonInteractStop?.Invoke(_currentInteractable);
        _currentInteractable.StopButtonInteract(_networkPlayer,out ButtonInteractionData interactionData);

        _isInteracting = false;
        if (interactionData.StopMovement)
        {
            _networkPlayer.Movement.CanMove = false;
        }

        if (interactionData.EnableMovement)
        {
            _networkPlayer.Movement.CanMove = true;
        }

        if (interactionData.OpenInventory)
        {
       //     if(!_networkPlayer.Inventory.IsOpen)
         //       _networkPlayer.Inventory.ToggleInventory();
        }
            
        if (interactionData.HideInventory)
        {
         //   if(_networkPlayer.Inventory.IsOpen)
           //     _networkPlayer.Inventory.ToggleInventory();
        }

        if (interactionData.EnableCameraLook)
        {
            _networkPlayer.Camera.enabled = true;
        }
            
        if (interactionData.StopCameraLook)
        {
            _networkPlayer.Camera.enabled = false;
        }
    }
}
