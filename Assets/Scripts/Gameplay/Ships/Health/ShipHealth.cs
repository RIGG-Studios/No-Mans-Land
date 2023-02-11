using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class ShipHealth : NetworkHealthHandler
{
    [Networked]
    public NetworkBool IsDead { get; private set; }
    
    private const byte StartingHealth = 100;

    private Ship _ship;

    protected override void Awake()
    {
        base.Awake();
        _ship = GetComponent<Ship>();
    }

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
            _ship.Physics.ReleaseGravity();
            SceneShipHandlerInstance.ShipHandler.RequestShipRespawn(_ship, _ship.TeamID);


            Debug.Log(_ship.Cannons.Length);
            foreach (CannonController controller in _ship.Cannons)
            {
                controller.OnSink();
            }
        }
    }

    public override bool Heal(float amount)
    {
        return false;
    }
}
