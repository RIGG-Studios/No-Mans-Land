using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[System.Serializable]
public struct ItemListData : INetworkStruct
{
    public int ItemID;
    public int SlotID;
    public int Stack;

    public ItemListData(int itemID, int slotID, int stack)
    {
        ItemID = itemID;
        SlotID = slotID;
        Stack = stack;
    }
}


public interface IInventory
{
    bool IsFull { get; }
    
    void AddItem(int itemID, int slotID = 0);
    void RemoveItem(int itemID);
    void UpdateItems(Item item, int newSlotID);

    void FindItem(int itemID, ref ItemListData itemData);
}
