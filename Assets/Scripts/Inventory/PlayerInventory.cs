using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : LocalInventory, IInputProccesor
{
    public bool IsSwitching { get; private set; }

    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private Transform weaponsRoot;

    public UnityEvent<bool> onInventoryToggled;
    public UnityEvent<int> onItemsChanged;


    public ItemController EquippedItem => Weapons[EquippedItemID];
    
    

    [Networked(OnChanged = nameof(OnEquippedItemChanged))]
    public int EquippedItemID { get; set; }
    
    [Networked, HideInInspector]
    public NetworkBool IsOpen { get; private set; }

    [Networked, HideInInspector]
    private NetworkButtons ButtonsPrevious { get; set; }

    [Networked, HideInInspector]
    public bool CanUse { get; set; }

    [Networked(OnChanged = nameof(OnWeaponsListChanged), OnChangedTargets = OnChangedTargets.All), Capacity(6)]
    public NetworkLinkedList<ItemController> Weapons { get; }

    private NetworkPlayer _player;

    protected override void Awake()
    {
        base.Awake();

        _player = GetComponent<NetworkPlayer>();
    }

    private static void OnEquippedItemChanged(Changed<PlayerInventory> changed)
    {
        changed.Behaviour.onItemsChanged?.Invoke(changed.Behaviour.EquippedItemID);
    }
    private static void OnWeaponsListChanged(Changed<PlayerInventory> changed)
    {
    }

    public override void Spawned()
    {
        base.Spawned();
        CanUse = true;

        if (!Object.HasStateAuthority)
        {
            return;
        }
        
        Item[] items = Context.ItemDatabase.GetEquippableItems();

        for (int i = 0; i < items.Length; i++)
        {
            NetworkObject obj = Runner.Spawn(items[i].fpPrefab, inputAuthority: Object.InputAuthority);

            ItemController itemController = obj.GetComponent<ItemController>();
            itemController.transform.SetParent(weaponsRoot, false);
            itemController.Init(i);
            itemController.gameObject.SetActive(false);
        
            Weapons.Add(itemController);
        }
    }
    
    public void ToggleInventory()
    {
        if (!CanUse)
        {
            return;
        }
        
        IsOpen = !IsOpen;

        if (Object.HasInputAuthority)
        {
            inventoryUI.SetActive(IsOpen);
            onInventoryToggled?.Invoke(IsOpen);
        }
    }
    
    private void SelectSlot(int i)
    {
        if (!CheckSlots(i, out int itemID))
        {
            return;
        }
        
        Debug.Log(Object.HasStateAuthority);
        
        ItemController itemController = FindItemController(itemID);

        if (itemController == null)
        {
            return;
        }

        SwitchItemControllers(itemController);
    }

    private bool CheckSlots(int j, out int itemID)
    {
        itemID = 0;
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].SlotID == SlotHandler.Slots[j].ID)
            {
                itemID = Items[i].ItemID;
                return true;
            }
        }

        return false;
    }
    
    public ItemControllerState GetEquippedItemState()
    {
        if (EquippedItem == null)
        {
            return default;
        }

        return EquippedItem.GetState();
    }

    public override void Render()
    {
        EquippedItem.transform.SetParent(weaponsRoot, false);
    }

    public override void OnSlotReset(Slot slot)
    {
        if (EquippedItem == null)
        {
            return;
        }
        
        
        if (slot.InventoryItem.ItemID == EquippedItem.item.itemID)
        {
            HideCurrentItem();
        }
    }
    
    private void SwitchItemControllers(ItemController nextController)
    {
        if (EquippedItem != null)
        {
            StartCoroutine(IE_SwitchItems(nextController));
        }
        else
        {
            nextController.gameObject.SetActive(true);
            nextController.Equip();
            EquippedItemID = nextController.ID;
        }
        
        if (Object.HasStateAuthority)
        {
            EquippedItemID = nextController.ID;
        }
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
        RPC_UpdateEquippedItemID(0);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_UpdateEquippedItemID(int id) => EquippedItemID = id;

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

        IsSwitching = false;
    }
    
    private ItemController FindItemController(int itemID)
    {
        Debug.Log(itemID);
        for (int i = 0; i < Weapons.Count; i++)
        {
            Debug.Log(Weapons[i].item.itemID);
            if (itemID == Weapons[i].item.itemID)
            {
                return Weapons[i];
            }
        }

        return null;
    }

    public void ProcessInput(NetworkInputData input)
    {
        NetworkButtons pressed = input.Buttons.GetPressed(ButtonsPrevious);
        NetworkButtons released = input.Buttons.GetReleased(ButtonsPrevious);

        if (pressed.IsSet(PlayerButtons.ToggleInventory))
        {
            ToggleInventory();
        }

        if (pressed.IsSet(PlayerButtons.Slot1))
        {
            SelectSlot(15);
        }
        
        if (pressed.IsSet(PlayerButtons.Slot1))
        {
            SelectSlot(16);
        }
        
        if (pressed.IsSet(PlayerButtons.Slot1))
        {
            SelectSlot(17);
        }
        
        if (pressed.IsSet(PlayerButtons.Slot1))
        {
            SelectSlot(18);
        }
        
        if (pressed.IsSet(PlayerButtons.Slot1))
        {
            SelectSlot(19);
        }

        if (EquippedItem != null)
        {
            EquippedItem.ProcessInput(input);
        }
    }
}
