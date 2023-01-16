using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

public abstract class NetworkInventory : ContextBehaviour, IInventory
{
    [SerializeField] protected int size;
    [SerializeField] protected Transform slotContainer;
    [SerializeField] private Item[] startingItems;
    
    public UnityEvent<ItemListData> onItemAdded;
    public UnityEvent<ItemListData> onItemRemoved;


    [Networked,Capacity(16)]
    public NetworkLinkedList<ItemListData> Items { get; } = new();
    
    protected SlotHandler SlotHandler;

    private bool _setup;

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

    private void Awake()
    {
        Slot[] slots = SlotSpawner.GenerateSlots(slotContainer, size);
        
        SlotHandler = new SlotHandler(this, slots);
        
        
        Invoke(nameof(AddItemDelay), 5.0f);
    }
    

    private void AddItemDelay()
    {
        if (Object.HasStateAuthority)
        {
            for (int i = 0; i < startingItems.Length; i++)
            {
                AddItem(startingItems[i].itemID, -1, startingItems[i].maxStack);
            }
        }
        else
        {
            RefreshInventory();
        }
    }
    
    public void AddItem(int itemID, int slotID = -1, int stack = 1, bool networked = false)
    {
        Slot slot = slotID != -1 ? SlotHandler.Slots[slotID] :  SlotHandler.GetNextSlot();

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

        ItemListData inventoryItem = new ItemListData(itemID, slot.ID, 1);
        
        slot.InitItem(item, ref inventoryItem);
        
        RequestAddItem(inventoryItem);
    }

    protected virtual void RequestAddItem(ItemListData itemListData) { }
    protected virtual void RequestRemoveItem(ItemListData itemListData) { }

    public void RefreshInventory()
    {
        Slot[] slots = SlotHandler.Slots;
        
        for (int z = 0; z < slots.Length; z++)
        {
            slots[z].Reset();
        }
        
        for (int i = 0; i < Items.Count; i++)
        {
            Slot slot = SlotHandler.FindSlotByID(Items[i].SlotID);
            Item item = Context.ItemDatabase.FindItem(Items[i].ItemID);

            var itemData = Items[i];
            slot.InitItem(item, ref itemData);
        }
    }

    public void RemoveItem(int itemID)
    {
        ItemListData inventoryItem = default;
        
        FindItem(itemID, out inventoryItem);

        Slot itemSlot = SlotHandler.FindSlotByID(inventoryItem.SlotID);
        
        itemSlot.Reset();
        RequestRemoveItem(inventoryItem);
    }

    public void OnSlotReset(Slot slot)
    {
        throw new NotImplementedException();
    }

    public void UpdateItems(int item, int newSlotID)
    {
     //   RequestUpdateItems(item.itemID, newSlotID);
    }

    protected virtual void RequestUpdateItems(int itemID, int newSlotID) { }
    

    public void ExecuteUpdateItems(int itemID, int newSlotID)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (itemID == Items[i].ItemID)
            {
                var inventoryItem = Items[i];
                inventoryItem.SlotID = newSlotID;
                Items.Set(i, inventoryItem);
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
}
