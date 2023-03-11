using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class RaycastAttacker : WeaponComponent, IAttacker
{
    [SerializeField] private float fireRate;
    [SerializeField] private float raycastLength;
    [SerializeField] private float damage;
    [SerializeField] private LayerMask attackableLayers;
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private BulletTracer bulletTracerPrefab;
    [SerializeField] private Transform barrel;
    [SerializeField] private AudioClip[] fireAudioClips;
    

    public event Action onAttack;
    
    private readonly int _fire = Animator.StringToHash("Fire");

    private int _fireTicks;
    
    [Networked]
    private TickTimer fireCooldown { get; set; }
    
    [Networked(OnChanged = nameof(OnAttack), OnChangedTargets = OnChangedTargets.All)]
    private NetworkBool _attack { get; set; }

    private AudioSource _audioSource;

    protected override void Awake()
    {
        base.Awake();
        _audioSource = GetComponent<AudioSource>();
    }
    
    public override void OnEnable()
    {
        base.OnEnable();
        
        Weapon.SetAttacker(this);
    }

    private static void OnAttack(Changed<RaycastAttacker> changed)
    {
        changed.LoadOld();
        bool oldAttack = changed.Behaviour._attack;
        
        changed.LoadNew();
        bool newAttack = changed.Behaviour._attack;
        
        if (!oldAttack && newAttack && !changed.Behaviour.Object.HasInputAuthority)
        {
            changed.Behaviour.PlaySFX();
        }
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
        if (Weapon.IsBusy() || !fireCooldown.ExpiredOrNotRunning(Runner) || !desires.HasAmmo || Weapon.Player.Movement.IsSprinting)
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
        _attack = !fireCooldown.ExpiredOrNotRunning(Runner);
        
        if (Object.HasInputAuthority && Runner.IsForward)
        {
            FireEffectsLocal();
        }

        Runner.LagCompensation.Raycast(context.FirePosition, context.FireDirection, raycastLength,
            Object.InputAuthority, out var hitInfo, attackableLayers, HitOptions.IncludePhysX);
        
        if (hitInfo.GameObject != null)
        {
            ImpactHandler.Instance.RequestImpact(hitInfo.GameObject.tag, hitInfo.Point, hitInfo.Normal);
        }
        
        
        if (hitInfo.Hitbox == null && hitInfo.Collider == null)
        {
            return;
        }

        Vector3 dir = (hitInfo.Point - Weapon.Player.Camera.transform.position).normalized;

        (HitData, bool) hitAttempt =
            NetworkDamageHandler.ProcessHit(Object.InputAuthority, dir, hitInfo, damage, HitAction.Damage);

        bool success = hitAttempt.Item2;
        HitData hitData = hitAttempt.Item1;

        if (Object.HasInputAuthority && Runner.IsForward)
        {
            if (hitData.IsFatal)
            {
                Weapon.Player.UI.EnableMenu("ScoreFeed");
                Weapon.Player.UI.GetService<ScoreFeed>().OnPlayerKilled(hitData.Victim.Owner.Owner.PlayerName.ToString(), "Musket");
            }


            if (hitInfo.Hitbox != null)
            {
                if (hitInfo.Hitbox.Root.GetComponent<INetworkDamagable>() != null)
                {
                    Weapon.Player.HitMarker.ShowHitMarker(hitData.IsFatal);
                }
            }
        }
        
        Weapon.OnFired();
    }

    public void Attack()
    {
     //   NetworkPlayer.Local.Attack.HitScanAttack(damage, raycastLength);
     //   onAttack?.Invoke();
        
     //   FireEffects();
    }

    private void PlaySFX()
    {
        _audioSource.PlayOneShot(fireAudioClips[Random.Range(0, fireAudioClips.Length)]);
    }
    
    private void FireEffectsLocal()
    {
        PlaySFX();
        Animator.SetTrigger(_fire);

        BulletTracer tracer = Instantiate(bulletTracerPrefab, barrel.transform.position, barrel.transform.rotation);
        tracer.Init(barrel.transform.forward);

        GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, barrel.position, barrel.rotation);
        muzzleFlash.GetComponent<VisualEffect>().Play();
        Destroy(muzzleFlash, 1.0f);
    }
}
