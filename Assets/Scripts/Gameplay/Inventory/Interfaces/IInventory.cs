using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[System.Serializable]
public struct ItemListData : INetworkStruct
{
    public int ID;
    public int ItemID;
    public int SlotID;
    public int Stack;

    public ItemListData(int id, int itemID, int slotID, int stack)
    {
        ID = id;
        ItemID = itemID;
        SlotID = slotID;
        Stack = stack;
    }
}

[System.Serializable]
public struct StartingItemData
{
    public Item item;
    public int stack;
}


public interface IInventory
{
    bool IsFull { get; }

    void AddItem(int itemID, int stack = 1, int slotID = -1);
    void RemoveItem(int itemID, int slotID = -1);
    
    void OnSlotHovered(Slot slot);
    void OnSlotUnHovered(Slot slot);
    void OnSlotReset(Slot slot);
    void UpdateItems(int oldSlotID, int newSlotID);
    void ThrowItem(Slot slot);
    
    bool FindItem(int itemID, out ItemListData itemData);
}
