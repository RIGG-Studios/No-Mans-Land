using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State 
{
    public enum StateTypes
    {
        Moving,
        Jumping,
        Swinging,
        Ladder,
        Sprinting
    }
    
    public StateTypes Type { get; private set; }

    protected PlayerMovementHandler MovementHandler;

    public virtual void Init(PlayerMovementHandler movementHandler, StateTypes type)
    {
        MovementHandler = movementHandler;
        Type = type;
    }
    
    public virtual void Enter() { }
    public virtual void Exit() { }

    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnFixedUpdateNetwork() { }

    public virtual void Move(NetworkInputData input) { }
    public virtual void Look(Vector2 lookDir) { }
}
