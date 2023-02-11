using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class ItemDatabase : ScriptableObject
{
    public Item[] items;


    public Item FindItem(int id)
    {
        Item item = null;

        for (int i = 0; i < items.Length; i++)
        {
            if (id != items[i].itemID)
            {
                continue;
            }

            item = items[i];
        }

        return item;
    }

    public Item[] GetEquippableItems()
    {
        List<Item> items = new();

        for (int i = 0; i < this.items.Length; i++)
        {
            if (!this.items[i].isEquippable)
            {
                continue;
            }
            
            items.Add(this.items[i]);
        }

        return items.ToArray();  
    }
}
