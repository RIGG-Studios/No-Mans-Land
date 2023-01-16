using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class BasicReload : WeaponComponent, IReloader
{
    [SerializeField] private bool autoReload;
    [SerializeField] private int maxCurrentAmmo;
    [SerializeField] private GameObject reloadText;
    [SerializeField] private Text ammoText;
    [SerializeField] private WeaponAnimationData reloadAnimationData;

    [Space] [SerializeField] private Item ammoItem;
    
    private static readonly int ReloadHash = Animator.StringToHash("Reload");

    public int CurrentAmmo { get; set; }
    public int ReserveAmmo { get; set; }
    public bool IsReloading { get; set; }


    public override void Awake()
    {
        base.Awake();

        CurrentAmmo = maxCurrentAmmo;
        
        ammoText.enabled = true;
        ammoText.text = CurrentAmmo + "/" + maxCurrentAmmo;
        
        InputActions.Player.Reload.performed += ctx => Reload();
    }

    public override void OnEquip()
    {
        if (CurrentAmmo <= 0 && NetworkPlayer.Local.Inventory.FindItem(ammoItem.itemID, out ItemListData itemData))
        {
            reloadText.SetActive(true);
        }

        ammoText.enabled = true;
    }

    public override void OnHide()
    {
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

        if (CurrentAmmo >= maxCurrentAmmo)
        {
            return;
        }

        StartCoroutine(IE_Reload());
    }

    private IEnumerator IE_Reload()
    {
        if (!NetworkPlayer.Local.Inventory.FindItem(ammoItem.itemID, out ItemListData itemData))
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
        
        NetworkPlayer.Local.Inventory.UpdateItemStack(itemData, ammoNeeded);
        CurrentAmmo += ammoNeeded;
        
        if (CurrentAmmo > 0)
        {
            ammoText.enabled = true;
            ammoText.text = CurrentAmmo + "/" + maxCurrentAmmo;
        }
        else
        {
            reloadText.SetActive(true);
        }

        IsReloading = false;
    }
}
