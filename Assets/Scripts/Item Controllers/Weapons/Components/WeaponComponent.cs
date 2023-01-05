using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponComponent : InputBase
{
    protected bool IsSetup;
    protected ModularWeapon Weapon;


    protected Animator Animator;

    public override void Awake()
    {
        base.Awake();

        Weapon = GetComponent<ModularWeapon>();
    }

    public virtual void OnEquip() {}
    public virtual void OnHide() { }
    
    public void Init(ModularWeapon weapon, Animator animator)
    {
        Animator = animator;
        IsSetup = true;
    }
}
