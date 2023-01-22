using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Fusion;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

public class Projectile : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }
    
    [Networked(OnChanged = nameof(OnDestroyChanged))]
    private bool Destroy { get; set; }

    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject projectileRender;
    [SerializeField] private VisualEffect hitImpact;

    private Transform _startTransform;

    public void Init(Transform startPos, Vector3 forward, float time = 5.0f)
    {
        life  = TickTimer.CreateFromSeconds(Runner, time);
        _startTransform = startPos;
        
        GetComponent<Rigidbody>().velocity = forward;
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
        {
            Runner.Despawn(Object);
            return;
        }
        
        DetectCollision();
    }

    private void DetectCollision()
    {
        if (Destroy)
        {
            return;
        }
        
        Collider[] hitColliders =
            Physics.OverlapBox(transform.position, transform.localScale / 2, quaternion.identity, layerMask);

        int i = 0;

        while (i < hitColliders.Length)
        {
          //  Debug.Log(hitColliders[i].gameObject.name);
            if (hitColliders[i].TryGetComponent(out INetworkDamagable damage))
            {
                if (Object.HasInputAuthority)
                {
                    DamageNotifier.Instance.OnDamageEntity(_startTransform, transform.position, 20f);
                }
                
                HitData hitData =
                    NetworkDamageHandler.ProcessHit(Object.InputAuthority, Vector3.zero, transform.position, 20f,
                        HitAction.Damage, damage);
                
                damage.ProcessHit(ref hitData);

                if (hitData.IsFatal && Object.HasInputAuthority)
                {
                }

                Destroy = true;
                Runner.Despawn(Object); 
                return;
            }
            i++;
        }
    }

    private void SinkShip()
    {
        
    }

    private static void OnDestroyChanged(Changed<Projectile> changed)
    {
        changed.Behaviour.projectileRender.SetActive(false);
        changed.Behaviour.hitImpact.Play();
    }
}
