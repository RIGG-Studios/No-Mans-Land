using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class LocalItemData
{
    public int SlotID;
    public int Stack;
    
    public Item Item;
    public ItemController ItemController;
}

public class LocalInventory : ContextBehaviour, IInventory
{
    [SerializeField] private LocInventoryContainer[] inventories;

    public UnityEvent<ItemListData> onItemAdded;
    public UnityEvent<ItemListData> onItemRemoved;

    protected SlotHandler SlotHandler;

    public List<ItemListData> Items { get; private set; } = new();


    [Serializable]
    public class LocInventoryContainer
    {
        public int id;
        public int size;
        public Transform grid;
        public Item[] startingItems;
    }
    
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

    public void InitSlots()
    {
        List<Slot> slots = new();

        for (int i = 0; i < inventories.Length; i++)
        {
            Transform slotContainer = inventories[i].grid;
            int size = inventories[i].size;
            
            Slot[] s = SlotSpawner.GenerateSlots(slotContainer, size);
            for (int z = 0; z < s.Length; z++)
            {
                slots.Add(s[z]);
            }
        }
        
        SlotHandler = new SlotHandler(this, slots.ToArray());
    }

    public virtual void Start()
    {
        for (int i = 0; i < inventories.Length; i++)
        {
            for (int z = 0; z < inventories[i].startingItems.Length; z++)
            {
                AddItem(inventories[i].startingItems[z].itemID);
            }
        }
    }
    

    public virtual void AddItem(int itemID, int slotID = -1)
    {
        Slot slot = slotID != -1 ? SlotHandler.Slots[slotID] :  SlotHandler.GetNextSlot();

        if (slot == null)
        {
            Debug.Log("Couldn't find slot, inventory is full or there was an error finding it");
            return;
        }

        Item item = SceneHandler.Instance.ItemDatabase.FindItem(itemID);

        if (item == null)
        {
            Debug.Log("Error finding item with ID: " + itemID);
            return;
        }

        ItemListData inventoryItem = new ItemListData(itemID, slot.ID, 1);
        
        slot.InitItem(item, ref inventoryItem);
        Items.Add(inventoryItem);
            onItemAdded?.Invoke(inventoryItem);
    }

    public virtual void RemoveItem(int itemID)
    {
        ItemListData itemData = default;
        FindItem(itemID, ref itemData);

        if (itemData.ItemID == 0)
        {
            Debug.Log("Error finding item");
            return;
        }

        Slot slot = SlotHandler.FindSlotByID(itemData.SlotID);
        
        slot.Reset();
        Items.Remove(itemData);
        onItemRemoved?.Invoke(itemData);
    }

    public void UpdateItems(Item item, int newSlotID)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (item.itemID == Items[i].ItemID)
            {
                var inventoryItem = Items[i];
                inventoryItem.SlotID = newSlotID;
                Items[i] = inventoryItem;
            }
        }
    }

    private void OnSlotReset(Slot slot)
    {
        
    }
    
    public void FindItem(int itemID, ref ItemListData itemData)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (itemID != Items[i].ItemID)
            {
                continue;
            }

            itemData = Items[i];
            return;
        }
    }
}
