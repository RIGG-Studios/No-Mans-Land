using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ParticleSystemJobs;

public class PlayerAttacker : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnAttackChanged))]
    public bool IsAttacking { get; set; }

    [Networked]
    public NetworkButtons ButtonsPrevious { get; set; }

    
    [SerializeField] private LayerMask attackableLayers;
    [SerializeField] private UnityEvent onAttacked;

    private NetworkPlayer _player;
    private float _lastAttack;
    
    private void Awake()
    {
        _player = GetComponent<NetworkPlayer>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData input))
        {
            return;
        }

        NetworkButtons pressed = input.Buttons.GetPressed(ButtonsPrevious);

        ButtonsPrevious = input.Buttons;
        
        
        if (pressed.IsSet(PlayerButtons.Fire) && !IsAttacking)
        {
        //    Attack();
        }
    }


    private void Attack()
    {
        if (Time.time - _lastAttack < 0.15f)
        {
            return;
        }
        
        Debug.Log(_player.Inventory.EquippedItem);
        if (Object.HasInputAuthority && _player.Inventory.EquippedItem != null)
        {
            _player.Inventory.EquippedItem.Attack();
        }
        
        IsAttacking = true;
        _lastAttack = Time.time;
        Invoke(nameof(ResetAttack), 0.09f);
    }

    public void HitScanAttack(float damage, float length)
    {

    }

    private void ResetAttack() => IsAttacking = false;

    private void OnAttackRemote()
    {
        onAttacked?.Invoke();
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
