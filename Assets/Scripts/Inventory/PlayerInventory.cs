using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : LocalInventory
{
    public bool IsOpen { get; private set; } = false;
    
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private Transform itemHolder;

    public UnityEvent<bool> onInventoryToggled;
    
    public void ToggleInventory()
    {
        IsOpen = !IsOpen;
        inventoryUI.SetActive(IsOpen);
        onInventoryToggled?.Invoke(IsOpen);
    }

    private void InitFPItems()
    {
        
    }

    public void SelectSlot(int i)
    {
        
    }
}
