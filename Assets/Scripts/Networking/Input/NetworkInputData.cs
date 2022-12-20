using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector2 MovementInput;
    public Quaternion LookForward;
    public NetworkBool IsJumpPressed;
    public NetworkBool IsSprintPressed;
}
