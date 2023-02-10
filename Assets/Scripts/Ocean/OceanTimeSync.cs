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
    
    protected override void Awake()
    {
        base.Awake();
        
        _waterSurface = FindFirstObjectByType<WaterSurface>();
        Debug.Log(_waterSurface);
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
