using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class Inventory : ContextBehaviour, IInventory
{
    [Networked(OnChanged = nameof(OnInventoryUpdated), OnChangedTargets = OnChangedTargets.All), Capacity(25)]
    public NetworkLinkedList<ItemListData> Items { get; }

    [SerializeField] private Transform slotContainer;
    [SerializeField] private StartingItemData[] startingItems;
    [SerializeField] private int size;

    protected SlotHandler SlotHandler;
    
    public bool IsFull
    {
        get
        {
            bool isFull = true;
            
            for (int i = 0; i < SlotHandler.Slots.Length; i++)
            {
                if (SlotHandler.Slots[i].HasItem)
                {
                    continue;
                }

                isFull = false;
            }

            return isFull;
        }
    }

    public override void Spawned()
    {
        Slot[] slots = SlotSpawner.GenerateSlots(slotContainer, size);
        SlotHandler = new SlotHandler(this, slots.ToArray());

        if (Object.HasStateAuthority)
        {
            for (int i = 0; i < startingItems.Length; i++)
            {
                AddItem(startingItems[i].item.itemID, startingItems[i].stack);
            }
        }
    }
    
    private static void OnInventoryUpdated(Changed<Inventory> changed)
    {
        changed.Behaviour.RefreshInventory();
    }
    
    protected void RefreshInventory()
    {
        for (int i = 0; i < SlotHandler.Slots.Length; i++)
        {
            SlotHandler.Slots[i].Reset();
        }
        
        for (int i = 0; i < Items.Count; i++)
        {
            for (int z = 0; z < SlotHandler.Slots.Length; z++)
            {
                if (Items[i].SlotID == SlotHandler.Slots[z].ID)
                {
                    Item item = Context.ItemDatabase.FindItem(Items[i].ItemID);
                    ItemListData itemData = Items[i];
                    
                    SlotHandler.Slots[z].InitItem(item, itemData);
                }
            }
        }
    }

    public virtual void AddItem(int itemID, int stack, int slotID = -1)
    {
        if (Object.HasStateAuthority)
        {
            AddItemExecute(itemID, stack, slotID);
        }
        else
        {
            RPC_AddItem(itemID, stack, slotID);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_AddItem(int itemID, int stack, int slotID = -1)
    {
        AddItemExecute(itemID, stack, slotID);
    }

    private void AddItemExecute(int itemID, int stack, int slotID = -1)
    {
        Slot slot = slotID != -1 ? SlotHandler.Slots[slotID] : SlotHandler.GetNextSlot();
        
        if (slot == null)
        {
            Debug.Log("Couldn't find slot, inventory is full or there was an error finding it");
            return;
        }

        Item item = Context.ItemDatabase.FindItem(itemID);

        if (item == null)
        {
            Debug.Log("Error finding item with ID: " + itemID);
            return;
        }
        
        ItemListData inventoryItem = new ItemListData(Items.Count+1, itemID, slot.ID, stack);
        slot.InitItem(item, inventoryItem);
        slot.HasItem = true;

        Items.Add(inventoryItem);
    }

    public virtual void RemoveItem(int itemID, int slotID = -1, int amountToAdd = 1)
    {
        if (Object.HasStateAuthority)
        {
            RemoveItemExecute(itemID, slotID);
        }
        else
        {
            RPC_RemoveItem(itemID, slotID);
        }
    }

    public void OnSlotHovered(Slot slot)
    {
        if (!Object.HasInputAuthority)
        {
            return;
        }
        
        NetworkPlayer.Local.Inventory.OnSlotHovered(slot);
    }

    public void OnSlotUnHovered(Slot slot)
    {
        if (!Object.HasInputAuthority)
        {
            return;
        }

        NetworkPlayer.Local.Inventory.OnSlotUnHovered(slot);
    }

    public void OnSlotReset(Slot slot) { }
    
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RemoveItem(int itemID, int slotID, int amountToRemove = 1)
    {
        RemoveItemExecute(itemID, slotID, amountToRemove);
    }

    private void RemoveItemExecute(int itemID, int slotID, int amountToRemove = 1)
    {
        ItemListData itemData = default;

        if (slotID != -1)
        {
            FindItemBySlotID(slotID, out itemData);
        }
        else
        {
            FindItem(itemID, out itemData);
        }

        
        if (itemData.ItemID == 0)
        {
            return;
        }
        
        Slot slot = SlotHandler.FindSlotByID(itemData.SlotID);
        slot.Reset();

        Items.Remove(itemData);
    }

    public void UpdateItems(int oldSlotID, int newSlotID)
    {
        RPC_RequestUpdateInventory(oldSlotID, newSlotID);
    }

    public void UpdateItemStack(int oldSlotID, int newSlotID)
    {
        throw new System.NotImplementedException();
    }

    public void ThrowItem(Slot slot) { }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestUpdateInventory(int oldSlotID, int newSlotID)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (oldSlotID == Items[i].SlotID)
            {
                var inventoryItem = Items[i];
                inventoryItem.SlotID = newSlotID;
                Items.Set(i, inventoryItem);
            }
        }
    }
    
    public void UpdateItemStack(ItemListData itemData, int amount = 1)
    {
        Slot slot = SlotHandler.FindSlotByID(itemData.SlotID);

        if (slot == null)
        {
            Debug.Log("Couldn't find item slot");
            return;
        }

        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].SlotID == itemData.SlotID)
            {
                ItemListData itm = Items[i];
                itm.Stack -= amount;

                Items.Set(i, itm);
                slot.UpdateItemStackText(Items[i].Stack);

                if (itm.Stack <= 0)
                {
                    RemoveItem(itm.ItemID);
                }
                break;
            }
        }
    }
    
    public bool FindItem(int itemID, out ItemListData itemData)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (itemID != Items[i].ItemID)
            {
                continue;
            }

            itemData = Items[i];
            return true;
        }

        itemData = default;
        return false;
    }
    
    public bool FindItemBySlotID(int slotID, out ItemListData itemData)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (slotID != Items[i].SlotID)
            {
                continue;
            }

            itemData = Items[i];
            return true;
        }

        itemData = default;
        return false;
    }
}
