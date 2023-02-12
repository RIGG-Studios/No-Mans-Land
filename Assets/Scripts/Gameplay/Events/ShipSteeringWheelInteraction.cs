using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class ShipSteeringWheelInteraction : NetworkBehaviour, IInteractable
{
    [SerializeField] private Ship ship;
    [SerializeField] private Transform LookTransform;

    public Ship Ship => ship;
    
    public string LookAtID =>  string.Format("<color={0}>[F]</color> INTERACT", "red");

    public string ID => "ShipWheel";
    public PlayerButtons ExitKey => PlayerButtons.Interact;


    public void LookAtInteract() { }

    public void StopLookAtInteract() { }
    
    
    public bool ButtonInteract(NetworkPlayer networkPlayer, out ButtonInteractionData interactData)
    {
        bool success = !(Ship == null);

        if (Ship.HasPilot)
        {
            interactData = default;
            success = false;
            return success;
        }

        Ship.RequestOwnership(networkPlayer.Object.InputAuthority);
        
        
        interactData = new ButtonInteractionData()
        {
            DisableCursor = true,
            StopMovement = true,
            HideInventory = true
        };


        return true;
    }

    public void StopButtonInteract(NetworkPlayer player, out ButtonInteractionData interactionData)
    {
        Ship.ResetOwnership();

        if (player.Object.HasInputAuthority)
        {
            player.Camera.UpdateHorizontalLock(0.0f, 0.0f, false);
        }
        
        interactionData = new ButtonInteractionData()
        {
            EnableMovement = true
        };
        
        interactionData.Interpolation = new InterpolationData()
        {
            IsValid = true,
            Return = true
        };
    }

}
