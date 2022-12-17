using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkInputManager : MonoBehaviour
{
    private PlayerMovementInputHandler _inputHandler;
    
    public void OnEnable()
    {
        NetworkCallBackEvents.onInput += OnInput;
    }

    public void OnDisable()
    {
        NetworkCallBackEvents.onInput -= OnInput;
    }
    
    private void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (_inputHandler == null && NetworkPlayer.Local != null)
        {
            _inputHandler = NetworkPlayer.Local.GetComponent<PlayerMovementInputHandler>();
        }

        if (_inputHandler != null)
        {
            input.Set(_inputHandler.GetNetworkInput());
        }
    }
}
