using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public struct TakeDamageData
{
    public float Damage;
    public PlayerRef Attacker;
    public byte NewHealth;

    public TakeDamageData(float damage, PlayerRef attacker, byte newHealth)
    {
        this.Damage = damage;
        this.Attacker = attacker;
        this.NewHealth = newHealth;
    }
}

public struct OnDeathData
{
    public string Attacker;

    public OnDeathData(string attacker)
    {
        Attacker = attacker;
    }
}

public abstract class NetworkHealthHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnHealthChanged))]
    public byte Health { get; set; }
    
    protected virtual void OnHealthReduced() { }

    public abstract void Damage(float damage, PlayerRef attackerRef);
    
    
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
