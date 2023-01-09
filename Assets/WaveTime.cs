using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class WaveTime : NetworkBehaviour
{
    public static WaveTime Instance;
    
    [Networked]
    public float Time { get; private set; }

    public override void Spawned()
    {
        Instance = this;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
        {
            return;
        }

        Time = UnityEngine.Time.time;
    }
}
