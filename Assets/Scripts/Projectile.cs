using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Fusion;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

public class Projectile : NetworkBehaviour
{
    [Networked] 
    private TickTimer life { get; set; }
    
    [Networked(OnChanged = nameof(OnDestroyChanged))]
    private bool Destroy { get; set; }

    [SerializeField] private float damageAmount;
    [SerializeField] private GameObject projectileRender;
    [SerializeField] private VisualEffect hitImpact;

    
    
    [Networked]
    private Vector3 startPosition { get; set; }

    public void Init(Transform startPos, Vector3 forward, float time = 5.0f)
    {
        life  = TickTimer.CreateFromSeconds(Runner, time);
        startPosition = startPos.position;
        
        GetComponent<Rigidbody>().velocity = forward;
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (Destroy)
        {
            return;
        }
        
        if (collision.gameObject.TryGetComponent(out INetworkDamagable damage))
        {
            Debug.Log(Object.HasInputAuthority);
            if (Object.HasInputAuthority)
            {
                DamageNotifier.Instance.OnDamageEntity(startPosition, transform.position, damageAmount);
            }
                
            HitData hitData =
                NetworkDamageHandler.ProcessHit(Object.InputAuthority, Vector3.zero, transform.position, damageAmount,
                    HitAction.Damage, damage);
                
            damage.ProcessHit(ref hitData); 
        }
        
        Destroy = true;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    private static void OnDestroyChanged(Changed<Projectile> changed)
    {
        changed.Behaviour.projectileRender.SetActive(false);
        changed.Behaviour.hitImpact.Play();
    }
}
