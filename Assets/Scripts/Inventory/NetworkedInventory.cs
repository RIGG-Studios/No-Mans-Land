using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkedInventory : IInventory
{
    public int size;
    public int ID { get; set; }
    public SlotHandler SlotsHandler { get; set; }

    public NetworkedInventory(int size, int id)
    {
        this.size = size;
        ID = id;
    }


    public void Init()
    {
        throw new System.NotImplementedException();
    }

    public void Destroy()
    {
    }

    public void AddItem(int itemID, int stack = 1)
    {
    }

    public void RemoveItem(int itemID, int amount = 1)
    {
    }
}
