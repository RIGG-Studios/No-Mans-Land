using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class Cannon : NetworkBehaviour, IInteractable
{
    public string LookAtID =>  string.Format("<color={0}>[F]</color> INTERACT", "red");
    public string ID => "Cannon";
    
    
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
        
        cannonController.RequestOccupyCannon(true, networkPlayer.Object.InputAuthority);
        
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
        if (Object.HasInputAuthority)
        {
            cannonController.RequestOccupyCannon(false, default);
        }
        

        interactionData = new ButtonInteractionData()
        {
            EnableMovement = true
        };
    }
}
