using Fusion;
using UnityEngine;

public enum Buttons
{
    Jump,
    Movement,
    Sprint,
    Fire,
    Reload
}

public struct NetworkInputData : INetworkInput
{
    public NetworkButtons Buttons;
    
    
    public Vector2 MovementInput;
    public Quaternion LookForward;
    public NetworkBool IsJumpPressed;
    public NetworkBool IsSprintPressed;
    
    
    public NetworkBool IsFirePressed;
    public NetworkBool IsReloadPressed;
}
