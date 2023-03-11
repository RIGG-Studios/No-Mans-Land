using Fusion;
using UnityEngine;

public enum PlayerButtons
{
    Jump,
    Sprint,
    Fire,
    Aim,
    Reload,
    Slot1,
    Slot2,
    Slot3,
    Slot4,
    Slot5,
    ToggleInventory,
    Escape,
    Interact,
    Enter
}

public enum PlayerStates : byte
{
    PlayerController,
    ShipController,
    CannonController
}

public struct NetworkInputData : INetworkInput
{
    public NetworkButtons Buttons;

    public Vector2 MovementInput;
    public Quaternion LookForward;
    public Quaternion LookVertical;
    
    public NetworkBool IsAiming;
    public NetworkBool IsReloading;
    public NetworkBool IsFiring;
    public NetworkBool IsSpacePressed;

    public float RawLookX;
    public float RawLookY;
    
    public PlayerStates CurrentState;
}