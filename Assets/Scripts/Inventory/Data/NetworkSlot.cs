using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSlot : Slot
{

    public override void Reset()
    {
        base.Reset();
        InventoryItem = default;
    }
}
