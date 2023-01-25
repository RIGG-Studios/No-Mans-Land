 using System.Numerics;
using Fusion;

public struct ItemControllerState
{
    public bool IsAiming;
    public bool IsReloading;

    public bool IsEquipping;
    public bool isHiding;
}

public struct ItemDesires
{
    public bool Fire;
    public bool Reload;
    public bool Aim;

    public bool HasAmmo;
    public bool HasFired;
}

public abstract class ItemController : ContextBehaviour
{
    public bool IsReady { get; set; }
    public NetworkPlayer Player { get; set; }

    public Item item;
    public float crossHairSize;

    [Networked]
    public int ID { get; set; }

    public void Init(int id)
    {
        ID = id;
    }

    
    
    public virtual ItemControllerState GetState()
    {
        return default;
    }

    public abstract T GetService<T>() where T : WeaponComponent;
    public virtual void ProcessInput(WeaponContext context) { }
    public virtual void Equip() { IsReady = true;}
    public virtual void Hide() { IsReady = false;}

    public virtual void Attack() { }

    public abstract float GetEquipTime();
    public abstract float GetHideTime();
}
