using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemController : MonoBehaviour
{
    public Item Item { get; private set; }
    public NetworkPlayer Player { get; private set; }

    private ItemListData _itemListData;
    
    public void Init(NetworkPlayer player, Item item, ref ItemListData itemListData)
    {
        Item = item;
        Player = player;
        _itemListData = itemListData;
    }

    public abstract void Equip();
    public abstract void Hide();

    public virtual void Attack() { }

    public abstract float GetEquipTime();
    public abstract float GetHideTime();
}
