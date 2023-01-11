using Fusion;
using UnityEngine;

public enum PlayerButtons
{
    Jump,
    Movement,
    Sprint,
    Fire,
    Reload
}

public enum ShipButtons
{
    ToggleCameraView
}

public struct NetworkInputData : INetworkInput
{
    public NetworkButtons Buttons;

    public Vector2 MovementInput;
    public Quaternion LookForward;
    
    //states
    public NetworkBool IsAiming;
    public NetworkBool IsReloading;

    //0 character controller
    //1 cannon controller
    //2 sailing controller
    public int CurrentState;

    public int CurrentWeaponID;
}