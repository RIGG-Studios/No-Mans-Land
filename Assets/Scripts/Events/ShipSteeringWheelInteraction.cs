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
    public string ID => "SteeringWheel";
    
    
    public void LookAtInteract() { }

    public void StopLookAtInteract() { }

    public bool ButtonInteract(NetworkPlayer networkPlayer)
    {
        if (Ship == null)
        {
            return false;
        }

        if (Ship.HasPilot)
        {
            return false;
        }

        Ship.RPC_RequestPilotChange(networkPlayer.Object.InputAuthority);
        return true;
    }

    public void StopButtonInteract()
    {
        Ship.RPC_RequestResetPilot();
    }
}
