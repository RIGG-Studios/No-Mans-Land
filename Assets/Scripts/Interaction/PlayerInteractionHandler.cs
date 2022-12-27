using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(PlayerInteractionInputHandler))]
public class PlayerInteractionHandler : MonoBehaviour
{
    public bool CanInteract { get; private set; }
    
    [SerializeField, Range(0, 5)] private float maxInteractionDistance;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private Transform lookFromPoint;
    [SerializeField] private GameObject interactUI;

    public UnityEvent<IInteractable> onLookAtInteract = new();
    public UnityEvent<IInteractable> onButtonInteract = new();
    public UnityEvent<IInteractable> onButtonInteractStop = new();

    private bool _sentLookAtEvent;

    private IInteractable _currentInteractable;

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

        RaycastHit hit;
        if (!Physics.Raycast(lookFromPoint.position, lookFromPoint.forward,
                out hit, maxInteractionDistance, interactLayer))
        {
            if (_currentInteractable != null)
            {
                _sentLookAtEvent = false;
                _currentInteractable.StopLookAtInteract();
                _currentInteractable = null;
                interactUI.SetActive(false);
            }

            return;
        }
        
        if (hit.transform.TryGetComponent(out IInteractable interactable))
        {
            _currentInteractable = interactable;
            
            if (!_sentLookAtEvent)
            {
                onLookAtInteract?.Invoke(_currentInteractable);
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
        
        bool success = _currentInteractable.ButtonInteract();

        if (success)
        {
            interactUI.SetActive(false);
            onButtonInteract?.Invoke(_currentInteractable);
        }
    }

    public void TryExitInteract()
    {
        if (_currentInteractable == null)
        {
            return;
        }
        
        _currentInteractable.StopLookAtInteract();
        _currentInteractable.StopButtonInteract();
        onButtonInteractStop?.Invoke(_currentInteractable);
    }
}
