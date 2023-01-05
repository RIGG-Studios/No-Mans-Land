using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class BasicReload : WeaponComponent, IReloader
{
    [SerializeField] private bool autoReload;
    [SerializeField] private int maxCurrentAmmo;
    [SerializeField] private GameObject reloadText;
    [SerializeField] private WeaponAnimationData reloadAnimationData;

    [Space] [SerializeField] private Item ammoItem;
    
    private static readonly int ReloadHash = Animator.StringToHash("Reload");

    public int CurrentAmmo { get; set; }
    public int ReserveAmmo { get; set; }
    public bool IsReloading { get; set; }


    public override void Awake()
    {
        base.Awake();

        InputActions.Player.Reload.performed += ctx => Reload();
    }
    
    public void OnFired()
    {
        DecrementCurrentAmmo();

        if (CurrentAmmo <= 0)
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

        StartCoroutine(IE_Reload());
    }

    private IEnumerator IE_Reload()
    {
        if (!Weapon.Player.Inventory.FindItem(ammoItem.itemID, out ItemListData itemData))
        {
            yield break;
        }
        
        reloadText.SetActive(false);

        IsReloading = true;

        Animator.SetTrigger(ReloadHash);

        yield return new WaitForSeconds(reloadAnimationData.ClipLength);

        int ammoNeeded = maxCurrentAmmo - CurrentAmmo;

        if (itemData.Stack < ammoNeeded)
        {
            ammoNeeded = itemData.Stack;
        }
        
        Weapon.Player.Inventory.UpdateItemStack(ref itemData, ammoNeeded);
        CurrentAmmo += ammoNeeded;

        IsReloading = false;
    }
}
