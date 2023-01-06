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

    public NetworkBool SailingShip;
}

public struct NetworkShipInputData : INetworkInput
{
    public ShipButtons Buttons;
    
    public Vector2 MovementInput;
}
