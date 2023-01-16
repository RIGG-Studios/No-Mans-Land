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

public enum HitFeedbackTypes : byte
{
    None,
    AnimatedHitMarker,
    AnimatedDamageText
}

public struct HitData
{
    public HitAction Action;
    public HitFeedbackTypes Feedback;
    public float Damage;
    public Vector3 Position;
    public Vector3 Direction;
    public Vector3 Normal;

    public PlayerRef AttackerRef;
    public INetworkDamagable Victim;
}

public interface INetworkInstigator
{
    PlayerRef OwnerRef { get; }

    void HitPerformed(HitData hit);
}

public interface INetworkDamagable
{
    bool IsActive { get; }

    PlayerRef OwnerRef { get; }
    
    bool ProcessHit(ref HitData hit);
}

public static class NetworkDamageHandler 
{
    public static HitData ProcessHit(PlayerRef attackerRef, Vector3 direction, LagCompensatedHit hit, float damage, HitAction hitAction, HitFeedbackTypes feedbackType)
    {
        INetworkDamagable networkDamagable = GetDamageTarget(hit.Hitbox, hit.Collider);

        if (networkDamagable == null)
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
            Feedback = feedbackType,
            Victim = networkDamagable,
            AttackerRef = attackerRef
        };

        return ProcessHit(ref hitData);
    }

    private static HitData ProcessHit(ref HitData hitData)
    {
        bool success = hitData.Victim.ProcessHit(ref hitData);
        
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
            return collider.GetComponentInChildren<INetworkDamagable>();
        }

        return null;
    }
}
