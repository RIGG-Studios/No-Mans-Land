using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRuntimeInventory
{
    void AddItem(int itemID, int amount = 1);
    void RemoveItem(int itemID, int amount = 1);
}
