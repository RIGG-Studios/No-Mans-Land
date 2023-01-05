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
        _lastAttack = Time.time;
        Invoke(nameof(ResetAttack), 0.09f);
    }

    public void HitScanAttack(float damage, float length)
    {
        LagCompensatedHit hitInfo =
            HitScanHandler.RegisterHitScan(Runner, Object, _player.Camera.transform, length, attackableLayers);

        if (hitInfo.Hitbox == null && hitInfo.Collider == null)
        {
            return;
        }
        
        
        Vector3 dir = (hitInfo.Point - _player.Camera.transform.position).normalized;

        HitData hitData =
            NetworkDamageHandler.ProcessHit(Runner.LocalPlayer, dir, hitInfo, damage, HitAction.Damage, HitFeedbackTypes.AnimatedDamageText);

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
}
