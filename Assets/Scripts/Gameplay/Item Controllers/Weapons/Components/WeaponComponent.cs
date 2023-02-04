using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class WeaponComponent : InputBase
{
    protected ModularWeapon Weapon;
    protected Animator Animator;

    public virtual bool IsBusy => false;


    public override void Awake()
    {
        base.Awake();

        Weapon = GetComponent<ModularWeapon>();
    }

    public virtual void OnEquip() {}
    public virtual void OnHide() { }
    
    public virtual void ProcessInput(WeaponContext context, ref ItemDesires desires) { }
    public virtual void FixedUpdateNetwork(WeaponContext context, ItemDesires desires) { }
    public virtual void OnRender(ItemDesires desires) { }

    
    public void Init(Animator animator)
    {
        Animator = animator;
    }
}
