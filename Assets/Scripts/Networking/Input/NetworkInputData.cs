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
    ToggleInventory
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
    public NetworkBool IsAiming;
    public NetworkBool IsReloading;

    public float RawLookX;
    public float RawLookY;
    
    public PlayerStates CurrentState;
    public int CurrentWeaponID;
}