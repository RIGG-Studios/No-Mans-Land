using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotHandler
{
    public Slot[] Slots;
    private int _size;
    private IInventory _inventory;

    public SlotHandler(IInventory inventory, Slot[] slots)
    {
        Slots = slots;
        _inventory = inventory;

        if (Slots == null)
        {
            return;
        }
        
        
        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i].InitSlot(_inventory,this, i);
            Slots[i].SlotReset += OnSlotReset;
        }
    }
    
    
    public Slot[] FindSelectedSlots()
    {
        List<Slot> slots = new();

        for (int i = 0; i < Slots.Length; i++)
        {
            if (!Slots[i].IsSelected)
            {
                continue;
            }
            
            slots.Add(Slots[i]);
        }

        return slots.ToArray();
    }


    public void SetSlots(Slot[] slots)
    {
        Slots = slots;
        
        for (int i = 0; i < Slots.Length; i++)
        {
            slots[i].InitSlot(_inventory,this, i);
        }
    }

    public void OnSlotReset(Slot slot)
    {
        _inventory.OnSlotReset(slot);
    }


    public Slot GetNextSlot()
    {
        Slot slot = null;

        for (int i = 0; i < Slots.Length; i++)
        {
            if (Slots[i].HasItem)
            {
                continue;
            }

            slot = Slots[i];
            break;
        }

        return slot;
    }

    public Slot FindSlotByID(int id)
    {
        Slot slot = null;

        for (int i = 0; i < Slots.Length; i++)
        {
            if (Slots[i].ID != id)
            {
                continue;
            }

            slot = Slots[i];
            break;
        }

        return slot;
    }
    

    public void ResetSlots()
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i].Reset();
        }
    }

    public void MoveItemInSlot(Slot oldSlot, Slot newSlot, bool toFirstSlot = false)
    {
        if (oldSlot == null || newSlot == null)
        {
            Debug.Log("Slots are null");
            return;
        }

        if (oldSlot == newSlot)
        {
            return;
        }

        if (newSlot.HasItem)
        {
            return;
        }
        
        Item item = SceneHandler.Instance.ItemDatabase.FindItem(oldSlot.InventoryItem.ItemID);

        
        if (oldSlot.InventoryItem.ItemID == newSlot.InventoryItem.ItemID && oldSlot.inventory != newSlot.inventory)
        {
            return;
        }

        if (oldSlot.InventoryItem.ItemID == newSlot.InventoryItem.ItemID)
        {
            //find inventory item
            //update the stack
            //if more then max stack, add another item with the remainder on server
            oldSlot.inventory.FindItem(item.itemID, out ItemListData inventoryItem);
            return;
        }
        
        if (oldSlot.inventory != newSlot.inventory)
        {
            oldSlot.inventory.FindItem(item.itemID, out ItemListData inventoryItem);
            oldSlot.inventory.RemoveItem(item.itemID, oldSlot.ID);
            newSlot.inventory.AddItem(item.itemID, inventoryItem.Stack, newSlot.ID);
            return;
        }

        _inventory.UpdateItems(oldSlot.ID, newSlot.ID);
    }
}
