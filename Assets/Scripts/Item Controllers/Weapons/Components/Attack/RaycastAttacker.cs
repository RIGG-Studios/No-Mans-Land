using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class RaycastAttacker : WeaponComponent, IAttacker
{
    [SerializeField] private float raycastLength;
    [SerializeField] private ParticleSystem[] muzzleFlash;
    [SerializeField] private float damage;

    public event Action onAttack;
    
    private readonly int _fire = Animator.StringToHash("Fire");

    private FPTerrainBlocker _blocker;

    public override void Awake()
    {
        base.Awake();

        _blocker = GetComponent<FPTerrainBlocker>();
    }
    
    public override void OnEnable()
    {
        base.OnEnable();
        
        Weapon.SetAttacker(this);
    }

    public void Attack()
    {
        if (Weapon.Reloader is {CurrentAmmo: <= 0})
        {
            return;
        }

        if (Weapon.Reloader is {IsReloading: true})
        {
            return;
        }

        if (_blocker.IsBlocked)
        {
            return;
        }

        Weapon.Player.Attack.HitScanAttack(damage, raycastLength);
        onAttack?.Invoke();
        
        FireEffects();
    }

    private void FireEffects()
    {
        foreach (ParticleSystem particle in muzzleFlash)
        {
            particle.Play();
        }
        
        Animator.SetTrigger(_fire);
    }
}
