using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalSlot : BaseSlot
{
    public LocalItemData InventoryItem { get; private set; }
    
    public override void InitLocalItem(LocalItemData itemData)
    {
        InventoryItem = itemData;

        slotIcon.sprite = itemData.Item.itemIcon;

        if (itemData.Item.IsStackable)
        {
            stackText.enabled = true;
        }
    }

    public override void Reset()
    {
        if (HasItem)
        {
            InventoryItem.ItemController.Hide();
        }

        HasItem = false;

        slotIcon.enabled = false;
        stackText.enabled = false;
        InventoryItem = null;
    }
}
