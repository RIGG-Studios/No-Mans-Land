using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class ShipSteeringWheelInteraction : ShipComponent, IInteractable
{
    public string LookAtID => "[F] INTERACT";
    public string ID => "SteeringWheel";
    
    
    public void LookAtInteract() { }

    public void StopLookAtInteract() { }

    public bool ButtonInteract()
    {
        if (Ship == null)
        {
            return false;
        }

        if (Ship.HasPilot)
        {
            return false;
        }
        
        
        return true;
    }

    public void StopButtonInteract()
    {
    }
}
