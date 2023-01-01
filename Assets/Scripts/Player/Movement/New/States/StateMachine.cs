using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class StateMachine : NetworkBehaviour
{
    public State CurrentState { get; private set; }


    public void EnterState(State.StateTypes stateType)
    {
        
    }
}
