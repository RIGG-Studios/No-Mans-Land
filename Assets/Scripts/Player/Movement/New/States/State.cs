using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State 
{
    public enum StateTypes
    {
        Moving,
        Jumping,
        Swinging,
        Ladder
    }
    
    public StateTypes Type { get; private set; }

    protected PlayerMovementHandler MovementHandler;

    public virtual void Init(PlayerMovementHandler movementHandler)
    {
        MovementHandler = movementHandler;
    }
    
    public abstract void Enter();
    public abstract void Exit();

    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnLateUpdate() { }

    public virtual void Move(Vector2 moveDir) { }
    public virtual void Look(Vector2 lookDir) { }
}
