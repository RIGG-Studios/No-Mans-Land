using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public abstract class NetworkHealthHandler : ContextBehaviour
{
    [Networked(OnChanged = nameof(OnHealthChanged))]
    public byte Health { get; set; }
    
    protected virtual void OnHealthReduced() { }

    public abstract bool Damage(HitData hitData);
    
    
    private static void OnHealthChanged(Changed<NetworkHealthHandler> changed)
    {
        byte newHP = changed.Behaviour.Health;
        
        changed.LoadOld();

        byte oldHP = changed.Behaviour.Health;

        if (newHP < oldHP)
        {
            changed.Behaviour.OnHealthReduced();
        }
    }

}
