using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class ShipSteeringWheelInteraction : NetworkBehaviour, IInteractable
{
    [SerializeField] private Ship ship;

    public Ship Ship => ship;
    
    public string LookAtID =>  string.Format("<color={0}>[F]</color> INTERACT", "red");

    public string ID => "ShipWheel";
    public PlayerButtons ExitKey => PlayerButtons.Escape;


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

    public void StopButtonInteract(NetworkPlayer player, out ButtonInteractionData interactionData)
    {
        Ship.RPC_RequestResetPilot();

        interactionData = new ButtonInteractionData()
        {
            EnableMovement = true
        };
    }

}
