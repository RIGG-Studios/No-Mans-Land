using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class StateMachine : ContextBehaviour
{
    public State.StateTypes CurrentStateType { get; private set; }
    public State CurrentState { get; private set; }

    protected MoveState MoveState;
    protected SprintState SprintState;
    protected JumpState JumpState;
    protected ShipSailingState SailingState;
    
    protected void InitStates(PlayerMovementHandler movementHandler)
    {
        MoveState = new MoveState();
        MoveState.Init(movementHandler, State.StateTypes.Moving);

        SprintState = new SprintState();
        SprintState.Init(movementHandler, State.StateTypes.Sprinting);

        JumpState = new JumpState();
        JumpState.Init(movementHandler, State.StateTypes.Jumping);

        SailingState = new ShipSailingState();
        SailingState.Init(movementHandler, State.StateTypes.Sailing);
    }

    private void Update()
    {
        CurrentState?.OnUpdate();
    }

    public void EnterState(State.StateTypes stateType)
    {
        State nextState = FindState(stateType);

        if (nextState == null)
        {
            Debug.Log("Error finding state");
            return;
        }

        
        Debug.Log("Entering state: " + stateType);
        CurrentState?.Exit();
        CurrentState = nextState;
        CurrentState.Enter();

        CurrentStateType = CurrentState.Type;
    }
    
    
    private State FindState(State.StateTypes stateType)
    {
        switch (stateType)
        {
            case State.StateTypes.Moving:
                return MoveState;
            
            case State.StateTypes.Sprinting:
                return SprintState;
            
            case State.StateTypes.Jumping:
                return JumpState;
            
            case State.StateTypes.Sailing:
                return SailingState;
        }

        return null;
    }
}
