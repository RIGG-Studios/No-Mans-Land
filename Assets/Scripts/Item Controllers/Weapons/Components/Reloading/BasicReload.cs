using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicReload : WeaponComponent, IReloader
{
    [SerializeField] private bool autoReload;
    [SerializeField] private int maxCurrentAmmo;
    [SerializeField] private WeaponAnimationData reloadAnimationData;

    [Space]
    
    [SerializeField] private int startingCurrentAmmo;
    [SerializeField] private int startingReserveAmmo;
    
    private static readonly int ReloadHash = Animator.StringToHash("Reload");

    public int CurrentAmmo { get; set; }
    public int ReserveAmmo { get; set; }
    public bool IsReloading { get; set; }

    public void OnFired()
    {
        DecrementCurrentAmmo();

        Debug.Log(CurrentAmmo);
        if (CurrentAmmo <= 0 && autoReload)
        {
            Reload();
        }
    }
    
    public override void OnEnable()
    {
        base.OnEnable();
        
        Weapon.SetReloader(this);
    }

    public void DecrementCurrentAmmo(int amount = 1)
    {
        CurrentAmmo -= amount;
    }

    public void IncrementCurrentAmmo(int amount = 1)
    {
        CurrentAmmo += amount;
    }

    public override void Awake()
    {
        base.Awake();

        CurrentAmmo = startingCurrentAmmo;
        ReserveAmmo = startingReserveAmmo;
        
      //  InputActions.Player
    }

    public void Reload()
    {
        if (IsReloading)
        {
            return;
        }

        StartCoroutine(IE_Reload());
    }

    private IEnumerator IE_Reload()
    {
        IsReloading = true;

        Animator.SetTrigger(ReloadHash);

        yield return new WaitForSeconds(reloadAnimationData.ClipLength);

        int ammoNeeded = maxCurrentAmmo - CurrentAmmo;

        if (ReserveAmmo < ammoNeeded)
        {
            ammoNeeded = ReserveAmmo;
        }
        
        ReserveAmmo -= ammoNeeded;
        CurrentAmmo += ammoNeeded;

        IsReloading = false;
    }
}
