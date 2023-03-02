using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class BasicReload : WeaponComponent, IReloader
{
    [SerializeField] private int maxCurrentAmmo;
    [SerializeField] private GameObject reloadText;
    [SerializeField] private Text ammoText;
    [SerializeField] private WeaponAnimationData reloadAnimationData;

    [Space] [SerializeField] private Item ammoItem;
    
    private static readonly int ReloadHash = Animator.StringToHash("Reload");

    
    [Networked]
    public int CurrentAmmo { get; set; }
    
    [Networked]
    public bool IsReloading { get; set; }


   // public virtual bool IsBusy => IsReloading;
    
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            ammoText.enabled = true;
            ammoText.text = CurrentAmmo + "/" + maxCurrentAmmo;
        }
        else
        {
            ammoText.enabled = false;
            reloadText.SetActive(false);
        }

        CurrentAmmo = maxCurrentAmmo;
    }

    public override void ProcessInput(WeaponContext context, ref ItemDesires desires)
    {
        if (Weapon.IsBusy())
        {
            return;
        }
        
        bool hasAmmo = Weapon.Player.Inventory.FindItem(ammoItem.itemID, out ItemListData itemData);
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
        
        if (CurrentAmmo <= 0 && NetworkPlayer.Local.Inventory.FindItem(ammoItem.itemID, out ItemListData itemData))
        {
            reloadText.SetActive(true);
        }

        ammoText.enabled = true;
    }

    public override void OnHide()
    {
        if (!Object.HasInputAuthority)
        {
            return;
        }
        
        reloadText.SetActive(false);
        ammoText.enabled = false;
    }
    
    public void OnFired()
    {
        DecrementCurrentAmmo();

        if (CurrentAmmo <= 0 && NetworkPlayer.Local.Inventory.FindItem(ammoItem.itemID, out ItemListData itemData))
        {
            reloadText.SetActive(true);
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

        if (CurrentAmmo <= 0)
        {
            ammoText.enabled = false;
        }
        else
        {
            ammoText.enabled = true;
            ammoText.text = CurrentAmmo + "/" + maxCurrentAmmo;
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
        if (!Weapon.Player.Inventory.FindItem(ammoItem.itemID, out ItemListData itemData))
        {
            yield break;
        }

        if (Object.HasInputAuthority && Runner.IsForward)
        {
            reloadText.SetActive(false);
            Animator.SetTrigger(ReloadHash);
        }
        
        IsReloading = true;
        
        yield return new WaitForSeconds(reloadAnimationData.ClipLength);

        int ammoNeeded = maxCurrentAmmo - CurrentAmmo;

        if (itemData.Stack < ammoNeeded)
        {
            ammoNeeded = itemData.Stack;
        }
        
        Weapon.Player.Inventory.UpdateItemStack(itemData, ammoNeeded);
        CurrentAmmo += ammoNeeded;

        if (Object.HasInputAuthority)
        {
            if (CurrentAmmo > 0)
            {
                ammoText.enabled = true;
                ammoText.text = CurrentAmmo + "/" + maxCurrentAmmo;
            }
            else
            {

                reloadText.SetActive(true);
            }
        }

        IsReloading = false;
    }
}
