using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class ShipHealth : NetworkHealthHandler
{
    [SerializeField] private ShipPhysicsHandler physicsHandler;
    
    [Networked]
    public NetworkBool IsDead { get; private set; }
    
    private const byte StartingHealth = 100;

    public override void Spawned()
    {
        if (!Object.HasStateAuthority)
        {
            return;
        }
        
        Health = StartingHealth;
    }

    public override bool Damage(ref HitData hitData)
    {
        Health -= (byte)hitData.Damage;
        Debug.Log($"{Time.time} {transform.name} took damage got {Health} left ");

        if (Health <= 0)
        {
            hitData.IsFatal = true;
            IsDead = true;
            Health = 0;
            
            Die();
        }
        
        return true;
    }

    private void Die()
    {
        if (Object.HasStateAuthority)
        {
            physicsHandler.ReleaseGravity();
        }
    }

    public override bool Heal(float amount)
    {
        return false;
    }
}
