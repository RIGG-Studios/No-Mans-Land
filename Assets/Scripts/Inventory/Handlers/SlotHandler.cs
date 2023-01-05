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

        if (oldSlot.Inventory != newSlot.Inventory)
        {
            if (newSlot.Inventory.IsFull)
            {
                return;
            }
            
            oldSlot.Inventory.RemoveItem(item.itemID);

            int slotID = toFirstSlot ? -1 : newSlot.ID;
            
            newSlot.Inventory.AddItem(item.itemID, slotID);
            return;
        }
        
        newSlot.InitItem(item, ref oldSlot.InventoryItem);
        oldSlot.InventoryItem.SlotID = newSlot.ID;
        oldSlot.Reset(); 
        _inventory.UpdateItems(item, newSlot.ID);
    }
}
