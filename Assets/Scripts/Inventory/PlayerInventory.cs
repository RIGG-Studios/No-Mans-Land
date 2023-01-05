using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : LocalInventory
{
    
    [SerializeField] private GameObject inventoryUI;

    public UnityEvent<bool> onInventoryToggled;
    
    
    public ItemController EquippedItem { get; private set; }
    public bool IsSwitching { get; private set; }
    public bool IsOpen { get; private set; }
    public bool CanUse { get; set; }
    
    
    private PlayerItemControllerHandler _itemControllerHandler;

    protected override void Awake()
    {
        base.Awake();

        _itemControllerHandler = GetComponent<PlayerItemControllerHandler>();
        CanUse = true;
    }

    public override void Start()
    {
        if (Object.HasInputAuthority)
        {
            InitSlots();
        }
        
        base.Start();
    }
    
    public void ToggleInventory()
    {
        if (!CanUse)
        {
            return;
        }
        
        IsOpen = !IsOpen;
        inventoryUI.SetActive(IsOpen);
        onInventoryToggled?.Invoke(IsOpen);
    }
    
    public void SelectSlot(int i)
    {
        if (!SlotHandler.Slots[i].HasItem)
        {
            return;
        }
        
        
        ItemListData data = SlotHandler.Slots[i].InventoryItem;
        ItemController itemController = FindItemController(data.ItemID);

        if (itemController == null)
        {
            return;
        }
        
        SwitchItemControllers(itemController, data.SlotID);
    }

    public override void OnSlotReset(Slot slot)
    {
        if (EquippedItem == null)
        {
            return;
        }
        
        if (slot.InventoryItem.ItemID == EquippedItem.Item.itemID)
        {
            HideCurrentItem();
        }
    }

    private void SwitchItemControllers(ItemController nextController, int slotID)
    {
        if (EquippedItem != null)
        {
            StartCoroutine(IE_SwitchItems(nextController));
        }
        else
        {
            nextController.gameObject.SetActive(true);
            nextController.Equip();
            EquippedItem = nextController;
        }
        
        SlotHandler.FindSlotByID(slotID).SelectSlot();
    }

    private void HideCurrentItem()
    {
        StartCoroutine(IE_HideCurrentItem());
    }

    private IEnumerator IE_HideCurrentItem()
    {
        EquippedItem.Hide();
        float hideTime = EquippedItem.GetHideTime();
        yield return new WaitForSeconds(hideTime);
        
        EquippedItem.gameObject.SetActive(false);
        EquippedItem = null;
    }

    private IEnumerator IE_SwitchItems(ItemController nextController)
    {
        IsSwitching = true;
        
        EquippedItem.Hide();
        float hideTime = EquippedItem.GetHideTime();
        yield return new WaitForSeconds(hideTime);
        
        EquippedItem.gameObject.SetActive(false);
        
        nextController.gameObject.SetActive(true);
        nextController.Equip();
        
        float equipTime = nextController.GetEquipTime();
        yield return new WaitForSeconds(equipTime);
        
        EquippedItem = nextController;
        IsSwitching = false;
    }
    
    private ItemController FindItemController(int itemID)
    {
        ItemController[] itemControllers = _itemControllerHandler.itemControllers.ToArray();
        
        for (int i = 0; i < itemControllers.Length; i++)
        {
            if (itemID == itemControllers[i].Item.itemID)
            {
                return itemControllers[i];
            }
        }

        return null;
    }
}
