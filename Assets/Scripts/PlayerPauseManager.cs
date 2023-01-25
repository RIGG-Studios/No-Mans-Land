using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerPauseManager : ContextBehaviour
{
    private NetworkPlayer _player;


    [Networked(OnChanged = nameof(OnOpenChanged), OnChangedTargets = OnChangedTargets.InputAuthority)] 
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
            _player.Camera.CanLook = !IsOpen;
        }
    }

    public void Disconnect()
    {
        Context.UI.FadeImage(1.0f, 2.0f);
        Context.Camera.Enable();
        Context.Gameplay.Disconnect(Runner);
    }

    private static void OnOpenChanged(Changed<PlayerPauseManager> changed)
    {
        if (changed.Behaviour.IsOpen)
        {
            changed.Behaviour._player.UI.EnableMenu("PauseMenu");
            changed.Behaviour.Context.Input.RequestCursorRelease();
        }
        else
        {
            changed.Behaviour._player.UI.DisableMenu("PauseMenu");
            changed.Behaviour.Context.Input.RequestCursorLock();
        }
    }
}
