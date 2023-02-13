using System.Collections;
using System.Collections.Generic;
using Fusion;
using JetBrains.Annotations;
using UnityEngine;

public class ShipVitalPoint : MonoBehaviour, INetworkDamagable
{
    [SerializeField] private ShipHealth shipHealth;
    
    public NetworkPlayer Owner { get; }
    public bool IsDead => shipHealth.IsDead;
    public INetworkDamagable.DamageTypes Type => INetworkDamagable.DamageTypes.Ship;

    
    public bool ProcessHit(ref HitData hit)
    {
        return shipHealth.Damage(ref hit);
    }
}
