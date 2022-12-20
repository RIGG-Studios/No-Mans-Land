using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public static class InventoryHandler
{
    [Networked, Capacity(16)] 
    public static NetworkLinkedList<IInventory> inventories { get; set; }

    public static IInventory InitInventory(int size, bool isNetworked, int id = 0)
    {
        id = GenerateUniqueID(); 
        inventories = new NetworkLinkedList<IInventory>();

        IInventory inventory = null;

        if (isNetworked)
        {
            inventory = new NetworkedInventory(size, id);
        }
        else
        {
            inventory = new Inventory(size, id);
        }

        inventories.Add(inventory);
        return inventory;
    }

    public static bool DestroyInventory(int id)
    {
        IInventory inventory = FindInventory(id);

        if (inventory == null)
        {
            Debug.Log("Error finding inventory with ID: " + id);
            return false;
        }

        inventories.Remove(inventory);
        return true;
    }


    public static IInventory FindInventory(int id)
    {
        for (int i = 0; i < inventories.Count; i++)
        {
            if (id == inventories[i].ID)
            {
                return inventories[i];
            }
        }

        return null;
    }

    private static int GenerateUniqueID()
    {
        return Random.Range(0, 50000);
    }
}
