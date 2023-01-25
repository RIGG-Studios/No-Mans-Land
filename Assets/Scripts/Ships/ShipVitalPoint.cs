using System.Collections;
using System.Collections.Generic;
using Fusion;
using JetBrains.Annotations;
using UnityEngine;

public class ShipVitalPoint : MonoBehaviour, INetworkDamagable
{
    [SerializeField] private NetworkHealthHandler shipHealth;
    
    public NetworkPlayer Owner { get; }
    public INetworkDamagable.DamageTypes Type => INetworkDamagable.DamageTypes.Ship;

    
    public bool ProcessHit(ref HitData hit)
    {
        return shipHealth.Damage(ref hit);
    }
}
