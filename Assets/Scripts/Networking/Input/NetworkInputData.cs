using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector2 MovementInput;
    public Vector2 LookForward;
    public NetworkBool IsJumpPressed;
}
