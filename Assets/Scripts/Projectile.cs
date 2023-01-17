using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.Mathematics;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }

    [SerializeField] private LayerMask layerMask;

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
        Collider[] hitColliders =
            Physics.OverlapBox(transform.position, transform.localScale / 2, quaternion.identity, layerMask);

        int i = 0;

        while (i < hitColliders.Length)
        {
          //  Debug.Log(hitColliders[i].gameObject.name);
            if (hitColliders[i].TryGetComponent(out ShipVitalPoint vitalPoint))
            {
                if (Object.HasInputAuthority)
                {
                    DamageNotifier.Instance.OnDamageEntity(_startTransform, transform.position, 20f);
                }

                Runner.Despawn(Object);
               return;
            }
            i++;
        }
    }
}
