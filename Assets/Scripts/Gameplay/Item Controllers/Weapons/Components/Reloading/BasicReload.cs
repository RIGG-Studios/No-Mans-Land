using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class BasicReload : WeaponComponent, IReloader
{
    [SerializeField] private int maxCurrentAmmo;
    [SerializeField] private int maxReserveAmmo;
    [SerializeField] private WeaponAnimationData reloadAnimationData;

    [Space] [SerializeField] private Item ammoItem;
    
    private static readonly int ReloadHash = Animator.StringToHash("Reload");

    
    [Networked]
    public int CurrentAmmo { get; set; }
    
    [Networked]
    public int CurrentReserveAmmo { get; set; }
    
    [Networked]
    public bool IsReloading { get; set; }


    public override bool IsBusy => IsReloading;
    
    public override void Spawned()
    {
        CurrentAmmo = maxCurrentAmmo;
        CurrentReserveAmmo = maxReserveAmmo;
    }
    
    


    public override void ProcessInput(WeaponContext context, ref ItemDesires desires)
    {
        if (Weapon.IsBusy())
        {
            return;
        }

        bool hasAmmo = CurrentReserveAmmo > 0;
        bool canReload = CurrentAmmo < maxCurrentAmmo;
        
        desires.HasAmmo = CurrentAmmo > 0;
        desires.Reload = hasAmmo && context.Input.Buttons.IsSet(PlayerButtons.Reload) && canReload;
    }

    public override void FixedUpdateNetwork(WeaponContext context, ItemDesires desires)
    {
        if (desires.HasFired)
        {
            DecrementCurrentAmmo();
        }

        if (desires.Reload)
        {
            Reload();
        }
    }

    public override void OnEquip()
    {
        if (!Object.HasInputAuthority)
        {
            return;
        }
        
        Weapon.Player.UI.UpdateAmmo(CurrentAmmo, CurrentReserveAmmo);
    }
    
    public void OnFired()
    {
        DecrementCurrentAmmo();
    }
    
    public override void OnEnable()
    {
        base.OnEnable();
        
        Weapon.SetReloader(this);
    }

    public void DecrementCurrentAmmo(int amount = 1)
    {
        CurrentAmmo -= amount;

        if (Object.HasInputAuthority && Runner.IsForward)
        {
            Weapon.Player.UI.UpdateAmmo(CurrentAmmo, CurrentReserveAmmo);
        }
    }

    public void IncrementCurrentAmmo(int amount = 1)
    {
        CurrentAmmo += amount;
    }
    
    public void Reload()
    {
        if (IsReloading)
        {
            return;
        }

        Weapon.Player.StartCoroutine(IE_Reload());
    }

    private IEnumerator IE_Reload()
    {
        if (CurrentReserveAmmo <= 0)
        {
            yield break;
        }

        if (Object.HasInputAuthority && Runner.IsForward)
        {
            Animator.SetTrigger(ReloadHash);
        }
        
        IsReloading = true;
        
        yield return new WaitForSeconds(reloadAnimationData.ClipLength);

        int ammoNeeded = maxCurrentAmmo - CurrentAmmo;

        if (CurrentReserveAmmo < ammoNeeded)
        {
            ammoNeeded = CurrentReserveAmmo;
        }
        
        CurrentAmmo += ammoNeeded;
        CurrentReserveAmmo -= ammoNeeded;

        if (Object.HasInputAuthority && Runner.IsForward)
        {
            Weapon.Player.UI.UpdateAmmo(CurrentAmmo, CurrentReserveAmmo);

        }

        IsReloading = false;
    }
}
