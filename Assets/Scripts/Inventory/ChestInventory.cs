using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class ChestInventory : NetworkBehaviour, IRuntimeInventory
{
    [SerializeField] private int size;
    [SerializeField] private bool syncSlotItems;

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
        RPCAddItem(itemID, amount);
    }

    public void RemoveItem(int itemID, int amount = 1)
    {
        RPCRemoveItem(itemID, amount);
    }

    public void OnSlotItemMoved()
    {
        RPCOnSlotMoved();
    }

    [Rpc]
    public void RPCOnSlotMoved()
    {
        
    }
    
    [Rpc]
    private void RPCAddItem(int itemID, int amount = 1)
    {
        _inventory.AddItem(itemID, amount);
    }

    [Rpc]
    private void RPCRemoveItem(int itemID, int amount = 1)
    {
        _inventory.RemoveItem(itemID, amount);
    }
}
