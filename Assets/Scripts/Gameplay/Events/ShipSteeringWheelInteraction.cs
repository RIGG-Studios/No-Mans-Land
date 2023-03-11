using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Assertions.Must;
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

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 dir = (networkPlayer.transform.position - transform.position).normalized;

        float dot = Vector3.Dot(forward, dir);
        Debug.Log(dot);

        bool success = !(Ship == null);

        if (Ship.HasPilot || dot < 0.88f)
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
