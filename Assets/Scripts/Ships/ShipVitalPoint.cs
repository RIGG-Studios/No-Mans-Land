using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ShipVitalPoint : MonoBehaviour, INetworkDamagable
{
    public void Damage(string attackerName, float dmg)
    {
        throw new System.NotImplementedException();
    }

    public bool IsActive { get; }
    public PlayerRef OwnerRef { get; }


    public NetworkPlayer Owner { get; }

    public bool ProcessHit(ref HitData hit)
    {
        throw new System.NotImplementedException();
    }
}
