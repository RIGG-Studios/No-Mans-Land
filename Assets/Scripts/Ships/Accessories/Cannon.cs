using Fusion;
using UnityEngine;

public class Cannon : NetworkBehaviour, IInteractable
{
    public string LookAtID =>  string.Format("<color={0}>[F]</color> INTERACT", "red");
    public string ID => "Cannon";

    private CannonController _cannonController;

    private void Awake()
    {
        _cannonController = GetComponent<CannonController>();
    }
    

    public void LookAtInteract() { }
    public void StopLookAtInteract() { }
    
    public bool ButtonInteract(NetworkPlayer networkPlayer, out ButtonInteractionData interactData)
    {
        interactData = default;
        if (_cannonController.isOccupied)
        {
            return false;
        }
        
        _cannonController.RPC_RequestOccupyCannon(true, networkPlayer.Object.InputAuthority);
        
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
            _cannonController.RPC_RequestOccupyCannon(false);
            _cannonController.Reset();
        }
        

        interactionData = new ButtonInteractionData()
        {
            EnableMovement = true
        };
    }

    

    private void SetupCannon()
    {
     //   cannonCamera.gameObject.SetActive(true);
    }
}
