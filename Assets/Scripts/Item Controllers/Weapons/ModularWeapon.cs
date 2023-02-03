using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ModularWeapon : BaseWeapon
{
    public IAimer Aimer { get; private set; }
    public IAttacker Attacker { get; private set; }
    public IRecoil CameraRecoil { get; private set; }
    public IRecoil WeaponRecoil { get; private set; }
    public IReloader Reloader { get; private set; }

    private WeaponComponent[] _weaponComponents;
    
    [Networked]
    private int lastFireTick { get; set; }
    
    private int _lastVisibleFireTick;
    
    private bool _test;

    public void SetAttacker(IAttacker attacker)
    {
        if (Attacker != null)
        {
            return;
        }
        
        Attacker = attacker; 
    }

    public void SetAimer(IAimer aimer)
    {
        if (Aimer != null)
        {
            return;
        }
        
        Aimer = aimer;
    }

    public void SetCameraRecoil(IRecoil recoil)
    {
        if (CameraRecoil != null)
        {
            return;
        }
        
        CameraRecoil = recoil;
        
    }

    public void SetWeaponRecoil(IRecoil recoil)
    {
        if (WeaponRecoil != null)
        {
            return;
        }
        
        WeaponRecoil = recoil;
        
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

    public override T GetService<T>()
    {
        for (int i = 0, count = _weaponComponents.Length; i < count; i++)
        {
            if (_weaponComponents[i] is T service)
                return service;
        }
        
        return null;
    }

    

    protected override void Awake()
    {
        base.Awake();
        Animator = GetComponentInChildren<Animator>();
        
        if (Animator == null)
        {
            Debug.Log("Couldn't find Animator");
        }
    }

    public override void Spawned()
    {
        _weaponComponents = GetComponents<WeaponComponent>();

        foreach (WeaponComponent component in _weaponComponents)
        {
            component.Init(Animator);
        }

        _lastVisibleFireTick = lastFireTick;
    }

    public bool IsBusy()
    {
        for (int i = 0; i < _weaponComponents.Length; i++)
        {
            if (_weaponComponents[i].IsBusy)
            {
                return true;
            }
        }

        return false;
    }

    public override void ProcessInput(WeaponContext context)
    {
        ItemDesires desires = default;

        for (int i = 0; i < _weaponComponents.Length; i++)
        {
            _weaponComponents[i].ProcessInput(context, ref desires);
        }
        
        for (int i = 0; i < _weaponComponents.Length; i++)
        {
            _weaponComponents[i].FixedUpdateNetwork(context, desires);
        }
    }

    public override void Render()
    {
        ItemDesires desires = default;

        desires.HasFired = _lastVisibleFireTick < lastFireTick &&
                           lastFireTick > Runner.Tick - (int)(Runner.Simulation.Config.TickRate * 0.5f);

        for (int i = 0; i < _weaponComponents.Length; i++)
        {
            _weaponComponents[i].OnRender(desires);
        }

        _lastVisibleFireTick = lastFireTick;
    }


    public override void Equip()
    {
        base.Equip();
        
        foreach (WeaponComponent component in _weaponComponents)
        {
            component.OnEquip();
        }
    }

    public void OnFired()
    {
        lastFireTick = Runner.Tick;
    }

    public override ItemControllerState GetState()
    {
        ItemControllerState state = default;
        
        if (Aimer is {IsAiming: true})
        {
            state.IsAiming = true;
        }

        if (Reloader is {IsReloading: true})
        {
            state.IsReloading = true;
        }
        
        
        state.IsEquipping = IsEquipping;
        state.isHiding = IsHiding;

        return state;
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
