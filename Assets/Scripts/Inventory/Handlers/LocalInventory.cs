using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Animations;

public class LocalInventory : MonoBehaviour, IInventory
{
    [SerializeField] private LocInventoryContainer[] inventories;

    private SlotHandler _slotHandler;

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
            
            for (int i = 0; i < _slotHandler.Slots.Length; i++)
            {
                if (_slotHandler.Slots[i].HasItem)
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
        
        _slotHandler = new SlotHandler(this, slots.ToArray());
    }

    private void Start()
    {
        for (int i = 0; i < inventories.Length; i++)
        {
            for (int z = 0; z < inventories[i].startingItems.Length; z++)
            {
                AddItem(inventories[i].startingItems[z].itemID);
            }
        }
    }
    

    public void AddItem(int itemID, int slotID = -1)
    {
        Slot slot = slotID != -1 ? _slotHandler.Slots[slotID] :  _slotHandler.GetNextSlot();

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
    }

    public void RemoveItem(int itemID)
    {
        ItemListData itemData = default;
        FindItem(itemID, ref itemData);

        if (itemData.ItemID == 0)
        {
            Debug.Log("Error finding item");
            return;
        }

        Slot slot = _slotHandler.FindSlotByID(itemData.SlotID);
        
        slot.Reset();
        Items.Remove(itemData);
    }

    public void UpdateItems(Item item, int newSlotID)
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
