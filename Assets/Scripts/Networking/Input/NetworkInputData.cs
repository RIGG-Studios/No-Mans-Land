using Fusion;
using UnityEngine;

public enum PlayerButtons
{
    Jump,
    Sprint,
    Fire,
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
    public Quaternion VerticalLook;
    
    public NetworkBool IsAiming;
    public NetworkBool IsReloading;

    public float RawLookX;
    public float RawLookY;
    
    public PlayerStates CurrentState;
    public int CurrentWeaponID;
}