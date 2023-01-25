using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class RaycastAttacker : WeaponComponent, IAttacker
{
    [SerializeField] private float fireRate;
    [SerializeField] private float raycastLength;
    [SerializeField] private float damage;
    [SerializeField] private LayerMask attackableLayers;
    [SerializeField] private VisualEffect muzzleFlash;

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

    public override void ProcessInput(WeaponContext context, ref ItemDesires desires)
    {
        if (Weapon.IsBusy() || !fireCooldown.ExpiredOrNotRunning(Runner) || !desires.HasAmmo)
        {
            return;
        }
        
        if (context.Input.Buttons.IsSet(PlayerButtons.Fire))
        {
            desires.HasFired = true;
        }
    }

    public override void FixedUpdateNetwork(WeaponContext context, ItemDesires desires)
    {
        if (!desires.HasFired)
        {
            return;
        }
        
        fireCooldown = TickTimer.CreateFromSeconds(Runner, _fireTicks / 2);
        
        if (Object.HasInputAuthority)
        {
            FireEffects();
        }

        
        Runner.LagCompensation.Raycast(context.FirePosition, context.FireDirection, raycastLength,
            Object.InputAuthority, out var hitInfo, attackableLayers, HitOptions.IncludePhysX);
        
        if (Object.HasStateAuthority && hitInfo.GameObject != null)
        {
            ImpactHandler.Instance.RequestImpact(hitInfo.GameObject.tag, hitInfo.Point, hitInfo.Normal);
        }
        
        
        if (hitInfo.Hitbox == null && hitInfo.Collider == null)
        {
            return;
        }

        if (hitInfo.Hitbox != null)
        {
            if (hitInfo.Hitbox.Root.GetComponent<INetworkDamagable>() != null && Object.HasInputAuthority)
            {
                Weapon.Player.HitMarker.ShowCrosshair();
            }
        }
        
        Vector3 dir = (hitInfo.Point - Weapon.Player.Camera.transform.position).normalized;

        HitData hitData =
            NetworkDamageHandler.ProcessHit(Object.InputAuthority, dir, hitInfo, damage, HitAction.Damage);
        
        
        if (hitData.IsFatal && Object.HasInputAuthority)
        {
            Weapon.Player.UI.ShowKillNotifcation(hitData.VictimUsername.ToString());
        }
        
        
        Weapon.OnFired();
    }

    public void Attack()
    {
        NetworkPlayer.Local.Attack.HitScanAttack(damage, raycastLength);
        onAttack?.Invoke();
        
        FireEffects();
    }

    private void FireEffects()
    {
        muzzleFlash.Play();
        Animator.SetTrigger(_fire);
    }
}
