using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponComponent : InputBase
{
    protected NetworkPlayer Player;
    protected ModularWeapon Weapon;
    protected Animator Animator;

    public virtual bool IsBusy => false;

    public virtual void OnEquip() {}
    public virtual void OnHide() { }
    
    public virtual void ProcessInput(NetworkInputData input, ref ItemDesires desires) { }
    public virtual void FixedUpdateNetwork(NetworkInputData input, ItemDesires desires) { }
    public virtual void OnRender() { }

    
    public void Init(NetworkPlayer player, ModularWeapon weapon, Animator animator)
    {
        Player = player;
        Weapon = weapon;
        Animator = animator;
    }
}
