using Fusion;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Serialization;

public class Cannon : NetworkBehaviour, IInteractable
{
    public string LookAtID =>  string.Format("<color={0}>[F]</color> INTERACT", "red");
    public string ID => "Cannon";
    public PlayerButtons ExitKey => PlayerButtons.Interact;


    [SerializeField] private CannonController cannonController;
    
    
    public void LookAtInteract() { }
    public void StopLookAtInteract() { }
    
    public bool ButtonInteract(NetworkPlayer networkPlayer, out ButtonInteractionData interactData)
    {
        interactData = default;
        if (cannonController.isOccupied)
        {
            return false;
        }

        
        if (networkPlayer.Object.HasInputAuthority)
        {
            float duration = ((cannonController.GetCannonCamera().transform.position -
                               networkPlayer.Camera.Camera.transform.position).magnitude / 5f );

     
            StartCoroutine(DelaySwapCamera(duration, networkPlayer, false, true));
        }
        
        cannonController.RequestOccupyCannon(true, networkPlayer.Object.InputAuthority);
        
        interactData = new ButtonInteractionData()
        {
            DisableCursor = true,
            StopMovement = true,
            HideInventory = true
        };

        interactData.Interpolation = new InterpolationData()
        {
            TargetPos = cannonController.GetCannonCamera().transform.position,
            TargetRot = cannonController.GetCannonCamera().transform.rotation,
            IsValid = true
        };
        
        return true;
    }

    public void StopButtonInteract(NetworkPlayer player, out ButtonInteractionData interactionData)
    {
        cannonController.RequestOccupyCannon(false, player.Object.InputAuthority);

        if (player.Object.HasInputAuthority)
        {
            player.Camera.ToggleCamera(true);
            cannonController.ToggleCamera(false);
            float duration = ((cannonController.GetCannonCamera().transform.position -
                               player.Camera.Camera.transform.position).magnitude / 5f );


            StartCoroutine(DelaySwapCamera(duration, player, true, false));
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

    public void DisableCamera(NetworkPlayer player)
    {
        player.Camera.ToggleCamera(true);
        cannonController.ToggleCamera(false);
    }

    public IEnumerator DelaySwapCamera(float time, NetworkPlayer player, bool playerCamOn, bool cannonCamOn)
    {
        yield return new WaitForSeconds(time);
        
               cannonController.ToggleCamera(cannonCamOn);
        player.Camera.ToggleCamera(playerCamOn);
    }
}
