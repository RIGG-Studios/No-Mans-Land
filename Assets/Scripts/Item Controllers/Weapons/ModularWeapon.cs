using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularWeapon : BaseWeapon
{
    public Item item;
    
    public IAimer Aimer { get; private set; }
    public IAttacker Attacker { get; private set; }
    public IRecoil CameraRecoil { get; private set; }
    public IRecoil WeaponRecoil { get; private set; }
    public IReloader Reloader { get; private set; }

    private WeaponComponent[] _weaponComponents;
    
    
    private Animator _weaponAnimator;
    private bool _test;

    public void SetAttacker(IAttacker attacker)
    {
        Attacker = attacker; 
    }

    public void SetAimer(IAimer aimer)
    {
        Aimer = aimer;
    }

    public void SetCameraRecoil(IRecoil recoil)
    {
        if (Attacker != null)
        {
            Attacker.onAttack += CameraRecoil.DoRecoil;
        }
        
        CameraRecoil = recoil;
    }

    public void SetWeaponRecoil(IRecoil recoil)
    {
        WeaponRecoil = recoil;

        if (Attacker != null)
        {
            Attacker.onAttack += WeaponRecoil.DoRecoil;
        }

        if (Aimer != null)
        {
            Aimer.onAim += WeaponRecoil.OnAimStateChanged;
        }
    }

    public void SetReloader(IReloader reloader)
    {
        Reloader = reloader;
        
        if (Attacker != null && !_test)
        {
            Attacker.onAttack += reloader.OnFired;
            _test = true;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _weaponAnimator = GetComponentInChildren<Animator>();

        if (_weaponAnimator == null)
        {
            Debug.Log("Couldn't find Animator");
        }
        
        _weaponComponents = GetComponents<WeaponComponent>();

        foreach (WeaponComponent component in _weaponComponents)
        {
            component.Init(this, _weaponAnimator);
        }
    }

    public override void Equip()
    {
        base.Equip();
        
        foreach (WeaponComponent component in _weaponComponents)
        {
            component.OnEquip();
        }
    }

    public override void Attack()
    {
        if (Attacker == null)
        {
            return;
        }
        
        Attacker.Attack();
    }

    public override void Hide()
    {
        base.Hide();
        
        foreach (WeaponComponent component in _weaponComponents)
        {
            component.OnHide();
        }
    }
}
