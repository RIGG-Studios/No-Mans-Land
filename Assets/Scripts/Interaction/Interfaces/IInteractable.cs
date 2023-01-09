public struct ButtonInteractionData
{
    public bool OpenInventory;
    public bool HideInventory;

    public bool EnableCursor;
    public bool DisableCursor;

    public bool StopMovement;
    public bool EnableMovement;

    public bool StopCameraLook;
    public bool EnableCameraLook;
}

public interface IInteractable
{ 
    string LookAtID { get; }
    string ID { get; }
    
    
    void LookAtInteract();

    void StopLookAtInteract();

    bool ButtonInteract(NetworkPlayer networkPlayer, out ButtonInteractionData interactData);
    void StopButtonInteract(out ButtonInteractionData interactionData);
}
