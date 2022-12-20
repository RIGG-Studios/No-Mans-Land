using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventory
{
    int ID { get; }
    SlotHandler SlotsHandler { get; }
    
    void Init();

    void Destroy();

    void AddItem(int itemID, int stack = 1);
    void RemoveItem(int itemID, int amount = 1);
}
