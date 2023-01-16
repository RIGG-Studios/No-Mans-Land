using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class RaycastAttacker : WeaponComponent, IAttacker
{
    [SerializeField] private float fireRate;
    [SerializeField] private float raycastLength;
    [SerializeField] private float damage;
    [SerializeField] private LayerMask attackableLayers;
    [SerializeField] private ParticleSystem[] muzzleFlash;

    public event Action onAttack;
    
    private readonly int _fire = Animator.StringToHash("Fire");

    private int _fireTicks;
    
    [Networked]
    private TickTimer fireCooldown { get; set; }
    
    public override void OnEnable()
    {
        base.OnEnable();
        
        Weapon.SetAttacker(this);
    }

    public override bool IsBusy => !fireCooldown.ExpiredOrNotRunning(Runner);

    public override void Spawned()
    {
        base.Spawned();
        
        float fireTime = 60f / fireRate;
        _fireTicks = (int)Math.Ceiling(fireTime / (double)Runner.DeltaTime);
    }

    public override void ProcessInput(NetworkInputData input, ref ItemDesires desires)
    {
        if (Weapon.IsBusy() || !fireCooldown.ExpiredOrNotRunning(Runner))
        {
            return;
        }
        
        if (input.Buttons.IsSet(PlayerButtons.Fire))
        {
            desires.HasFired = true;
        }
    }

    public override void FixedUpdateNetwork(NetworkInputData input, ItemDesires desires)
    {
        if (!desires.HasFired)
        {
            return;
        }

        fireCooldown = TickTimer.CreateFromSeconds(Runner, _fireTicks);
        
        
        if (Object.HasInputAuthority)
        {
            FireEffects();

        }
        
        LagCompensatedHit hitInfo =
            HitScanHandler.RegisterHitScan(Runner, Object, Player.Camera.transform, raycastLength, attackableLayers);

        if (hitInfo.Hitbox == null && hitInfo.Collider == null || !Object.HasStateAuthority)
        {
            return;
        }
        
        
        Vector3 dir = (hitInfo.Point - Player.Camera.transform.position).normalized;

        HitData hitData =
            NetworkDamageHandler.ProcessHit(Runner.LocalPlayer, dir, hitInfo, damage, HitAction.Damage, HitFeedbackTypes.AnimatedDamageText);

        if (hitData.Action != HitAction.None)
        {
            //local effects
        }
    }

    public void Attack()
    {
        NetworkPlayer.Local.Attack.HitScanAttack(damage, raycastLength);
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
