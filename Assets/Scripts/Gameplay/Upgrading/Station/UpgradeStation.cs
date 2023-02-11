using Fusion;
using UnityEngine;

public class UpgradeStation : NetworkBehaviour, IInteractable
{
    [SerializeField] private Transform rotationPoint;
    [SerializeField] private Transform upgradeCamera;
    [SerializeField] private float rotateSpeed;
    
    public string LookAtID { get; }
    public string ID => "Upgrade Station";
    public PlayerButtons ExitKey => PlayerButtons.Interact;

    [Networked]
    public NetworkBool isOccupied { get; set; }


    public void LookAtInteract() { }
    public void StopLookAtInteract() { }


    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData input))
        {
            return;
        }

        float rotateAmt = input.RawLookX * rotateSpeed;
        upgradeCamera.RotateAround(rotationPoint.position, transform.up, rotateAmt);
    }

    public bool ButtonInteract(NetworkPlayer networkPlayer, out ButtonInteractionData interactData)
    {
        if (networkPlayer.Object.HasInputAuthority)
        {
            networkPlayer.Camera.ToggleCamera(false);
            upgradeCamera.gameObject.SetActive(true);
        }

        if (Object.HasStateAuthority)
        {
            isOccupied = true;
            Object.AssignInputAuthority(networkPlayer.Object.InputAuthority);
        }

        interactData = new ButtonInteractionData()
        {
            EnableCursor = true,
            StopMovement = true,
            HideInventory = true
        };
        return true;
    }

    public void StopButtonInteract(NetworkPlayer networkPlayer, out ButtonInteractionData interactionData)
    {
        if (Object.HasStateAuthority)
        {
            isOccupied = false;
            Object.AssignInputAuthority(default);
        }
        
        if (networkPlayer.Object.HasInputAuthority)
        {
            networkPlayer.Camera.ToggleCamera(true);
            upgradeCamera.gameObject.SetActive(false);
        }
        
        interactionData = default;
    }
}
