using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public abstract class InventoryBase : NetworkBehaviour
{

    public abstract void AddItem(int itemID, int amount = 1);
    public abstract void RemoveItem(int itemID, int amount = 1);
}
