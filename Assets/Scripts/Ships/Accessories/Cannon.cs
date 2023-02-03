using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class Cannon : NetworkBehaviour, IInteractable
{
    public string LookAtID =>  string.Format("<color={0}>[F]</color> INTERACT", "red");
    public string ID => "Cannon";
    public PlayerButtons ExitKey => PlayerButtons.Escape;


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

    public void StopButtonInteract(NetworkPlayer player, out ButtonInteractionData interactionData)
    {
        cannonController.RequestOccupyCannon(false, player.Object.InputAuthority);

        interactionData = new ButtonInteractionData()
        {
            EnableMovement = true
        };
    }
}
