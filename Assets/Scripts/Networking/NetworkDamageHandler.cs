using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public enum HitAction : byte
{
    None,
    Damage,
    Heal
}

public enum HitTypes : byte
{
    Player,
    Ship,
    Other
}

public struct HitData
{
    public HitAction Action;
    public float Damage;
    public Vector3 Position;
    public Vector3 Direction;
    public Vector3 Normal;
    public bool IsFatal;

    public INetworkDamagable Victim;

    public PlayerRef Attacker;
    public NetworkString<_16> VictimUsername;

}

public interface INetworkDamagable
{
    public enum DamageTypes
    {
        Player,
        Ship
    }
    
    DamageTypes Type { get; }
    NetworkPlayer Owner { get; }
    
    bool ProcessHit(ref HitData hit);
}

public static class NetworkDamageHandler 
{
    public static HitData ProcessHit(PlayerRef attackerRef, Vector3 direction, LagCompensatedHit hit, float damage, HitAction hitAction)
    {
        INetworkDamagable networkDamagable = GetDamageTarget(hit.Hitbox, hit.Collider);

        
        if (networkDamagable == null)
        {
            return default;
        }

        if (networkDamagable.Owner == null)
        {
            return default;
        }

        if (attackerRef == networkDamagable.Owner.Object.InputAuthority)
        {
            return default;
        }

        HitData hitData = new HitData()
        {
            Action = hitAction,
            Damage = damage,
            Direction = direction,
            Position = hit.Point,
            Normal = hit.Normal,
            Victim = networkDamagable,
            Attacker = attackerRef,
            VictimUsername = networkDamagable.Owner.Owner.PlayerName
        };

        return ProcessHit(ref hitData);
    }
    
    public static HitData ProcessHit(PlayerRef attackerRef, Vector3 direction, Vector3 position, float damage, HitAction hitAction, INetworkDamagable networkDamagable)
    {
        if (networkDamagable == null)
        {
            return default;
        }

        HitData hitData = new HitData()
        {
            Action = hitAction,
            Damage = damage,
            Direction = direction,
            Position = position,
            Victim = networkDamagable,
            Attacker = attackerRef,
        };

        return ProcessHit(ref hitData);
    }

    private static HitData ProcessHit(ref HitData hitData)
    {
        bool success = hitData.Victim.ProcessHit(ref hitData);
        
        Debug.Log(hitData.IsFatal);
        return hitData;
    }

    private static INetworkDamagable GetDamageTarget(Hitbox hitbox, Collider collider)
    {
        if (hitbox != null)
        {
            return hitbox.Root.GetComponent<INetworkDamagable>();
        }

        if (collider != null)
        {
            return collider.transform.root.GetComponent<INetworkDamagable>();
        }

        return null;
    }
}
