using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class ShipSteeringWheelInteraction : ShipComponent, IInteractable
{
    [SerializeField] private Ship ship;

    public Ship Ship => ship;
    
    public string LookAtID => "[F] INTERACT";
    public string ID => "ShipWheel";
    
    
    public void LookAtInteract() { }

    public void StopLookAtInteract() { }
    
    
    public bool ButtonInteract(NetworkPlayer networkPlayer, out ButtonInteractionData interactData)
    {
        bool success = !(Ship == null);

        if (Ship.HasPilot)
        {
            success = false;
        }

        Ship.RPC_RequestPilotChange(networkPlayer.Object.InputAuthority);
        
        interactData = new ButtonInteractionData()
        {
            DisableCursor = true,
            StopMovement = true,
            HideInventory = true
        };

        return true;
    }

    public void StopButtonInteract(out ButtonInteractionData interactionData)
    {
        Ship.RPC_RequestResetPilot();

        interactionData = new ButtonInteractionData()
        {
            EnableMovement = true
        };
    }

    public void StopButtonInteract()
    {
        Ship.RPC_RequestResetPilot();
    }
}
