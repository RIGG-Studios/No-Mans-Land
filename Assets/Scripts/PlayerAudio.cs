using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerAudio : SimulationBehaviour
{
    [SerializeField] private AudioReverbZone underwaterEffects;

    private NetworkPlayer _player;

    private void Awake()
    {
        _player = GetComponent<NetworkPlayer>();
    }

    private void Update()
    {
        if (!Object.HasInputAuthority)
        {
            return;
        }
        
        underwaterEffects.gameObject.SetActive(_player.Movement.CameraSubmerged);
    }
}
