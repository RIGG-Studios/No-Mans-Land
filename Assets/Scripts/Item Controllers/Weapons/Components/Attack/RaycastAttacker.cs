using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class RaycastAttacker : WeaponComponent, IAttacker
{
    [SerializeField] private float raycastLength;
    [SerializeField] private float damage;

    public event Action onAttack;
    
    private readonly int _fire = Animator.StringToHash("Fire");

    public override void OnEnable()
    {
        base.OnEnable();
        
        Weapon.SetAttacker(this);
    }

    public void Attack()
    {
        Weapon.Player.Attack.HitScanAttack(damage, raycastLength);
        
        Animator.SetTrigger(_fire);
        onAttack?.Invoke();
    }
}
