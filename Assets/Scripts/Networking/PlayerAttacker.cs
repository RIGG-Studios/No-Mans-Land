using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

public class PlayerAttacker : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnAttackChanged))]
    public bool IsAttacking { get; set; }

    
    [SerializeField] private LayerMask attackableLayers;

    private NetworkPlayer _player;
    private float _lastAttack;
    
    private void Awake()
    {
        _player = GetComponent<NetworkPlayer>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData inputData))
        {
            return;
        }

        if (inputData.IsFirePressed && !IsAttacking)
        {
            Attack();
        }
    }


    private void Attack()
    {
        if (Time.time - _lastAttack < 0.15f)
        {
            return;
        }
        
        if (Object.HasInputAuthority)
        {
            _player.Inventory.EquippedItem.Attack();
        }
        
        IsAttacking = true;
        Invoke(nameof(ResetAttack), 0.09f);
    }

    public void HitScanAttack(float damage, float length)
    {
        LagCompensatedHit hitInfo =
            HitScanHandler.RegisterHitScan(Runner, Object, _player.Camera.transform, length, attackableLayers);

        Vector3 dir = (hitInfo.Point - _player.Camera.transform.position).normalized;

        HitData hitData = NetworkDamageHandler.ProcessHit(Runner.LocalPlayer, dir, hitInfo, damage, HitAction.Damage);

        if (hitData.Action != HitAction.None)
        {
            //local effects
        }
    }

    private void ResetAttack() => IsAttacking = false;

    private void OnAttackRemote()
    {
        
    }
    
    private static void OnAttackChanged(Changed<PlayerAttacker> changed)
    {
        bool isAttackingNow = changed.Behaviour.IsAttacking;
        
        changed.LoadOld();

        bool isAttackingPrevious = changed.Behaviour.IsAttacking;

        if (isAttackingNow && !isAttackingPrevious)
        {
            changed.Behaviour.OnAttackRemote();
        }
    }
    
            
    /*/
    Transform raycastTransform = _player.Camera.transform;

    Runner.LagCompensation.Raycast(raycastTransform.position, raycastTransform.forward, length,
        Object.InputAuthority, out var hitInfo, attackableLayers, HitOptions.IncludePhysX);

    float hitDist = 100f;

    if (hitInfo.Hitbox != null)
    {
        
        if(Object.HasInputAuthority) DamageNotifier.Instance.OnDamageEntity(hitInfo.Point, 34);

        if (Object.HasStateAuthority)
        {
            hitInfo.Hitbox.GetComponent<IDamageable>().Damage(_player.PlayerName.ToString(), 20f);
        }
        else
        {
            
        }
    }
    else if (hitInfo.Collider != null)
    {
        //hit physics collider
    }
    else
    {
        //hit nothing
    }

    _lastAttack = Time.time;
    /*/
}
