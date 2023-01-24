using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerPauseManager : ContextBehaviour
{
    private NetworkPlayer _player;


    [Networked] 
    public NetworkBool IsOpen { get; set; }

    protected override void Awake()
    {
        base.Awake();
        _player = GetComponent<NetworkPlayer>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData input))
        {
            return;
        }

        if (input.Buttons.IsSet(PlayerButtons.Escape))
        {
            IsOpen = !IsOpen;
        }


        if (IsOpen)
        {
            
        }
    }
}
