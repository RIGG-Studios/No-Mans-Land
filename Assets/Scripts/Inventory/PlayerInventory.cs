using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : LocalInventory
{
    [SerializeField] private GameObject inventoryUI;

    public UnityEvent<bool> onInventoryToggled;
    public UnityEvent<int> onItemsChanged;


    public ItemController EquippedItem { get; private set; }
    public bool IsSwitching { get; private set; }
    public bool IsOpen { get; private set; }
    public bool CanUse { get; set; }


    [Networked(OnChanged = nameof(OnEquippedItemChanged))]
    public int EquippedItemID { get; set; }
    
    public int RequestedEquippedItem { get; private set; }


    private PlayerItemControllerHandler _itemControllerHandler;

    protected override void Awake()
    {
        base.Awake();

        _itemControllerHandler = GetComponent<PlayerItemControllerHandler>();
        CanUse = true;
    }

    public static void OnEquippedItemChanged(Changed<PlayerInventory> changed)
    {
        Debug.Log(changed.Behaviour.EquippedItemID);
        changed.Behaviour.onItemsChanged?.Invoke(changed.Behaviour.EquippedItemID);
    }

    public override void Start()
    {
        if (Object.HasInputAuthority)
        {
            InitSlots();
        }
        
        base.Start();
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData input))
        {
            return;
        }

        EquippedItemID = input.CurrentWeaponID;
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
    
    public ItemControllerState GetEquippedItemState()
    {
        if (EquippedItem == null)
        {
            return default;
        }

        return EquippedItem.GetState();
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
        RequestedEquippedItem = nextController.Item.itemID;
    }

    private void HideCurrentItem()
    {
        StartCoroutine(IE_HideCurrentItem());
        RequestedEquippedItem = -1;
    }

    private IEnumerator IE_HideCurrentItem()
    {
        EquippedItem.Hide();
        float hideTime = EquippedItem.GetHideTime();
        yield return new WaitForSeconds(hideTime);
        
        EquippedItem.gameObject.SetActive(false);
        EquippedItem = null;
        EquippedItemID = -1;
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
