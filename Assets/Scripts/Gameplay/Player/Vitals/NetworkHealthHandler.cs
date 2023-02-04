using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public abstract class NetworkHealthHandler : ContextBehaviour
{
    [Networked(OnChanged = nameof(OnHealthChanged))]
    public byte Health { get; set; }
    
    protected virtual void OnHealthReduced() { }
    protected virtual void OnHealthIncreased() { }

    public abstract bool Damage(ref HitData hitData);
    public abstract bool Heal(float amount);
    
    
    private static void OnHealthChanged(Changed<NetworkHealthHandler> changed)
    {
        byte newHP = changed.Behaviour.Health;
        
        changed.LoadOld();

        byte oldHP = changed.Behaviour.Health;

        if (newHP < oldHP)
        {
            changed.Behaviour.OnHealthReduced();
        }

        if (oldHP < newHP)
        {
            changed.Behaviour.OnHealthIncreased();
        }
    }

}
