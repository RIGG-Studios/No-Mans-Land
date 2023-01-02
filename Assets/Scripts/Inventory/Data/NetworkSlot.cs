using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSlot : BaseSlot
{
    public ItemListData InventoryItem { get; private set; }
    
    public override void InitNetworkItem(Item item, ref ItemListData itemData)
    {
        InventoryItem = itemData;
        
        slotIcon.sprite = item.itemIcon;

        if (item.IsStackable)
        {
            stackText.enabled = true;
        }
    }

    public override void Reset()
    {
        HasItem = false;

        slotIcon.enabled = false;
        stackText.enabled = false;
        InventoryItem = default;
    }
}
