using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Inventory : IInventory
{
    public int ID { get; set; }
    public SlotHandler SlotsHandler { get; }
    public int size;
    
    [Networked, Capacity(16)]
    public NetworkLinkedList<InventoryItem> Items { get; set; }

    public Inventory(int id, int size)
    {
        ID = id;
        this.size = size;
    }
    
    public void Init()
    {
        throw new System.NotImplementedException();
    }

    public void Destroy()
    {
        throw new System.NotImplementedException();
    }

    public void AddItem(int itemID, int stack = 1)
    {
        throw new System.NotImplementedException();
    }

    public void RemoveItem(int itemID, int amount = 1)
    {
        throw new System.NotImplementedException();
    }
}
