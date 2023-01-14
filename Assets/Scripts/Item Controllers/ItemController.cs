using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ItemControllerState
{
    public bool IsAiming;
    public bool IsReloading;

    public bool IsEquipping;
    public bool isHiding;
}

public abstract class ItemController : MonoBehaviour
{
    public Item Item { get; private set; }
    public NetworkPlayer Player { get; private set; }

    
    public void Init(NetworkPlayer player, Item item)
    {
        Item = item;
        Player = player;
    }

    public abstract void Equip();
    public abstract void Hide();

    public virtual void Attack() { }

    public virtual ItemControllerState GetState()
    {
        return default;
    }

    public abstract float GetEquipTime();
    public abstract float GetHideTime();
}
