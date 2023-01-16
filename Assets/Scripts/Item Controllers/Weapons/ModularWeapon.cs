using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularWeapon : BaseWeapon
{
    public IAimer Aimer { get; private set; }
    public IAttacker Attacker { get; private set; }
    public IRecoil CameraRecoil { get; private set; }
    public IRecoil WeaponRecoil { get; private set; }
    public IReloader Reloader { get; private set; }

    private WeaponComponent[] _weaponComponents;
    
    
    private Animator _weaponAnimator;
    private NetworkPlayer _player;
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

        if (Attacker != null)
        {
            Attacker.onAttack += CameraRecoil.DoRecoil;
        }
    }

    public void SetWeaponRecoil(IRecoil recoil)
    {
        if (WeaponRecoil != null)
        {
            return;
        }
        
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
        _player = transform.root.GetComponent<NetworkPlayer>();
        if (_weaponAnimator == null)
        {
            Debug.Log("Couldn't find Animator");
        }
        
        _weaponComponents = GetComponents<WeaponComponent>();
    }

    public override void Spawned()
    {
        foreach (WeaponComponent component in _weaponComponents)
        {
            component.Init(_player, this, _weaponAnimator);
        }
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

    public override void ProcessInput(NetworkInputData input)
    {
        ItemDesires desires = default;

        for (int i = 0; i < _weaponComponents.Length; i++)
        {
            _weaponComponents[i].ProcessInput(input, ref desires);
        }
        
        for (int i = 0; i < _weaponComponents.Length; i++)
        {
            _weaponComponents[i].FixedUpdateNetwork(input, desires);
        }
    }

    public override void OnRender()
    {
        for (int i = 0; i < _weaponComponents.Length; i++)
        {
            _weaponComponents[i].OnRender();
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
