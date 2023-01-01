using UnityEngine;

public struct WeaponDesires
{
    public bool AmmoAvailable;
    public bool Fire;
    public bool HasFired;
    public bool Reload;
}

[RequireComponent(typeof(WeaponAction))]
public abstract class WeaponBaseComponent : ContextBehaviour
{
    public byte WeaponActionID { get; set; }
    
    public Weapon Weapon { get; set; }

    public virtual bool IsBusy => false;


    public abstract void ProcessInput(NetworkInputData context, ref WeaponDesires desires, bool isBusy);
    public abstract void OnFixedUpdate();
    public virtual void OnRender(ref WeaponDesires desires) { }


    protected void HasFired()
    {
    //    WeaponAction weaponAction = Weapon
    }
}
