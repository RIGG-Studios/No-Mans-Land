using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


[RequireComponent(typeof(PlayerInteractionInputHandler))]
public class PlayerInteractionHandler : MonoBehaviour
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

    private bool _sentLookAtEvent;
    private bool _isInteracting;
    private IInteractable _currentInteractable;

    private NetworkPlayer _networkPlayer;

    private void Awake()
    {
        _networkPlayer = GetComponent<NetworkPlayer>();
    }

    private void Start()
    {
        CanInteract = true;
    }

    private void Update()
    {
        if (!CanInteract)
        {
            return;
        }

        if (_isInteracting)
        {
            return;
        }

        RaycastHit hit;
        if (!Physics.Raycast(lookFromPoint.position, lookFromPoint.forward,
                out hit, maxInteractionDistance, interactLayer))
        {
            if (_currentInteractable != null)
            {
                _sentLookAtEvent = false;
                _currentInteractable.StopLookAtInteract();
                interactText.text = "";
                _currentInteractable = null;
                interactUI.SetActive(false);
            }
            return;
        }
        
        if (hit.collider.TryGetComponent(out IInteractable interactable))
        {
            _currentInteractable = interactable;
            
            if (!_sentLookAtEvent)
            {
                onLookAtInteract?.Invoke(_currentInteractable);
                interactText.text = _currentInteractable.LookAtID;
                interactUI.SetActive(true);
                _sentLookAtEvent = true;
            }
        }
        else
        {
            if (_currentInteractable != null)
            {
                _sentLookAtEvent = false;
                _currentInteractable.StopLookAtInteract();
                interactText.text = "";
                _currentInteractable = null;
                interactUI.SetActive(false);
            }
        }
    }

    public void TryButtonInteract()
    {
        if (_currentInteractable == null)
        {
            return;
        }
        
        bool success = _currentInteractable.ButtonInteract(NetworkPlayer.Local, out ButtonInteractionData interactionData);

        _isInteracting = success;
        
        if (success)
        {
            interactUI.SetActive(false);
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
                if(!_networkPlayer.Inventory.IsOpen)
                    _networkPlayer.Inventory.ToggleInventory();
            }
            
            if (interactionData.HideInventory)
            {
                if(_networkPlayer.Inventory.IsOpen)
                    _networkPlayer.Inventory.ToggleInventory();
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

    public void TryExitInteract()
    {
        if (_currentInteractable == null)
        {
            return;
        }
        
        _currentInteractable.StopLookAtInteract();
        onButtonInteractStop?.Invoke(_currentInteractable);
        _currentInteractable.StopButtonInteract(out ButtonInteractionData interactionData);

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
            if(!_networkPlayer.Inventory.IsOpen)
                _networkPlayer.Inventory.ToggleInventory();
        }
            
        if (interactionData.HideInventory)
        {
            if(_networkPlayer.Inventory.IsOpen)
                _networkPlayer.Inventory.ToggleInventory();
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
