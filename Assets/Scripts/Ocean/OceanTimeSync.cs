using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class OceanTimeSync : ContextBehaviour
{
    [Networked]
    private float waterTimeMultiplier { get; set; }

    private WaterSurface _waterSurface;

    public override void Spawned()
    {
        _waterSurface = FindFirstObjectByType<WaterSurface>();
    }

    public override void FixedUpdateNetwork()
    {
        if (Context.Session.SessionState == Session.SessionStates.Gameplay)
        {
            waterTimeMultiplier = 1f;
        }

        _waterSurface.timeMultiplier = waterTimeMultiplier;
    }
}
