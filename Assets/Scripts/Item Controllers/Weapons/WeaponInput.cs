using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInput : InputBase
{
    private ModularWeapon _modularWeapon;
    
    public override void Awake()
    {
        base.Awake();

        _modularWeapon = GetComponent<ModularWeapon>();
    }

    private void Start()
    {
        InputActions.Player.Aim.performed += ctx => _modularWeapon.Aim();
        InputActions.Player.Fire.performed += ctx => _modularWeapon.Attack();
    }
}
