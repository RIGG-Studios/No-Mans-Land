using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkWater : NetworkBehaviour
{
    [Networked]
    private float WaterTime { get; set; }
    
    private void Update()
    {
        if (!Object.HasStateAuthority)
        {
            return;
        }

        WaterTime += Time.time * 0.5f + 0.5f;
    }
}
