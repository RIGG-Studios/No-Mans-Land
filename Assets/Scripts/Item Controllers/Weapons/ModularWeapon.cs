using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularWeapon : BaseWeapon
{
    public enum WeaponStates
    {
        None,
        Idle,
        Equipping,
        Reloading,
        Hiding
    }
    
    public WeaponStates WeaponState { get; private set; }
    
    public Item item;

    private Animator _weaponAnimator;

    public IAimer Aimer { get; private set; }
    public IAttacker Attacker { get; private set; }
    public IRecoil CameraRecoil { get; private set; }
    public IRecoil WeaponRecoil { get; private set; }
    public IReloader Reloader { get; private set; }

    private bool _test;

    public void SetAttacker(IAttacker attacker)
    {
        Debug.Log("att");
        Attacker = attacker; 
    } 
    public void SetAimer(IAimer aimer) => Aimer = aimer;

    public void SetCameraRecoil(IRecoil recoil)
    {
        if (Attacker != null)
        {
            Debug.Log("a9tt");
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
        
        WeaponComponent[] components = GetComponents<WeaponComponent>();

        foreach (WeaponComponent component in components)
        {
            component.Init(this, _weaponAnimator);
        }
    }

    public void Aim()
    {

    }

    public override void Attack()
    {
        if (Attacker == null)
        {
            return;
        }
        
        Attacker.Attack();
    }

    public void ChangeState(WeaponStates state)
    {
        if (WeaponState == state)
        {
            return;
        }

        WeaponState = state;
    }
}
