using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour, IRuntimeInventory
{
    [SerializeField] private int size;

    public UnityEvent<InventoryItem> onItemAdded;
    public UnityEvent<InventoryItem> onItemRemoved;
    public UnityEvent<bool> onInventoryToggled;
    
    private IInventory _inventory;

    private void Start()
    {
        IInventory inventory = InventoryHandler.InitInventory(size, false);

        if (inventory != null)
        {
            _inventory = inventory;
        }
    }

    public void AddItem(int itemID, int amount = 1)
    {
        _inventory.AddItem(itemID, amount);
    }

    public void RemoveItem(int itemID, int amount = 1)
    {
        _inventory.RemoveItem(itemID, amount);
    }
}
