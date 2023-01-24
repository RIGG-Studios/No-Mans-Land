using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class LocalInventory : ContextBehaviour, IInventory
{
    [SerializeField] private LocInventoryContainer[] inventories;
    [SerializeField] private Transform throwTransform;
    [SerializeField] private GameObject itemInfoParent;
    [SerializeField] private Image itemSprite;
    [SerializeField] private Text itemName;
    [SerializeField] private Text itemDescription;

    public UnityEvent<ItemListData> onItemAdded;
    public UnityEvent<ItemListData> onItemRemoved;

    protected SlotHandler SlotHandler;


    [Networked(OnChanged = nameof(OnInventoryUpdated), OnChangedTargets = OnChangedTargets.InputAuthority), Capacity(25)]
    public NetworkLinkedList<ItemListData> Items { get; }
    
    
    private bool _skipRefresh = true;

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

    /// <summary>
    /// in this method we need to find out which slots changed, and only update them
    /// </summary>
    /// <param name="changed"></param>
    private static void OnInventoryUpdated(Changed<LocalInventory> changed)
    {
        changed.LoadOld();
        ItemListData[] oldItems = changed.Behaviour.Items.ToArray();
        
        changed.LoadNew();
        ItemListData[] newItems = changed.Behaviour.Items.ToArray();

        List<ItemListData> changedSlots = new();


        for (int i = 0; i < oldItems.Length; i++)
        {
            for (int z = 0; z < newItems.Length; z++)
            {
                if (oldItems[i].ID == newItems[z].ID && (oldItems[i].SlotID != newItems[z].SlotID))
                {
                    changedSlots.Add(newItems[z]);
                    changedSlots.Add(oldItems[i]);
                }
            }
        }

        changed.Behaviour.RefreshInventory(changedSlots.ToArray());
    }
    
    private void RefreshInventory(ItemListData[] changedItems)
    {
        for (int i = 0; i < changedItems.Length; i++)
        {
            Slot slot = SlotHandler.FindSlotByID(changedItems[i].SlotID);
            slot.Reset();
        }

        
        for (int i = 0; i < Items.Count; i++)
        {
            for (int z = 0; z < SlotHandler.Slots.Length; z++)
            {
                if (Items[i].SlotID == SlotHandler.Slots[z].ID)
                {
                    Item item = Context.ItemDatabase.FindItem(Items[i].ItemID);
                    ItemListData itemData = Items[i];
                    
                    SlotHandler.Slots[z].InitItem(item, ref itemData);
                }
            }
        }
    }
    

    public override void Spawned()
    {
        InitSlots();

        if (!Object.HasStateAuthority)
        {
            return;
        }
        
        for (int i = 0; i < inventories.Length; i++)
        {
            for (int z = 0; z < inventories[i].startingItems.Length; z++)
            {
                AddItem(inventories[i].startingItems[z].itemID, -1, inventories[i].startingItems[z].maxStack, false);
            }
        }
    }
    

    public virtual void AddItem(int itemID, int slotID = -1, int stack = 1, bool networked = true)
    {
        if (networked)
        {
            RPC_AddItem(itemID, slotID, stack);
        }
        else
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
            slot.InitItem(item, ref inventoryItem);
            slot.HasItem = true;

            Items.Add(inventoryItem);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_AddItem(int itemID, int slotID, int stack)
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

        ItemListData inventoryItem = new ItemListData(Items.Count+1, itemID, slot.ID, stack);

        slot.HasItem = true;
        Items.Add(inventoryItem);
    }

    public virtual void RemoveItem(int itemID)
    {
        ItemListData itemData = default;
        FindItem(itemID, out itemData);

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

    public void OnSlotHovered(Slot slot)
    {
        Item item = Context.ItemDatabase.FindItem(slot.InventoryItem.ItemID);

        if (item != null)
        {
            itemInfoParent.SetActive(true);
            itemSprite.sprite = item.itemIcon;
            itemName.text = item.itemName;
            itemDescription.text = item.itemDescription;
        }
    }

    public void OnSlotUnHovered(Slot slot)
    {
        itemInfoParent.SetActive(false);
        itemSprite.sprite = null;
        itemName.text = "";
        itemDescription.text = "";
    }

    public virtual void OnSlotReset(Slot slot)
    {
    }

    public void UpdateItems(int oldSlotID, int newSlotID)
    {
        RPC_RequestUpdateInventory(oldSlotID, newSlotID);
    }

    public void ThrowItem(Slot slot)
    {
        if (!slot.HasItem)
        {
            return;
        }

        RPC_ThrowItem(slot.InventoryItem.ItemID, slot.InventoryItem.Stack);

    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_ThrowItem(int itemID, int stack = 1)
    {
        Item item = Context.ItemDatabase.FindItem(itemID);

        if (item != null)
        {
            ItemPickup pickup = Runner.Spawn(item.pickupPrefab, throwTransform.position, throwTransform.rotation)
                .GetComponent<ItemPickup>();

            if (pickup != null)
            {
                pickup.Init(itemID, stack);
                RemoveItem(itemID);
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
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

                _skipRefresh = false;
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
}
