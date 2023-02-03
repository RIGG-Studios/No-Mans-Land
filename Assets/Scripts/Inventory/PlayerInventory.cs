using System.Collections;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class WeaponContext
{
    public NetworkInputData Input;

    public Vector3 FireDirection;
    public Vector3 FirePosition;
}

public class PlayerInventory : LocalInventory, IInputProccesor
{
    public bool IsSwitching { get; private set; }
    
    public UnityEvent<int> onItemsChanged;

    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private Transform fireTransform;
    [SerializeField] private Transform weaponsRoot;


    public ItemController EquippedItem => Weapons[EquippedItemID];
    
    

    [Networked(OnChanged = nameof(OnEquippedItemChanged), OnChangedTargets = OnChangedTargets.All)]
    public int EquippedItemID { get; set; }
    
    [Networked]
    public NetworkBool IsOpen { get; private set; }

    [Networked, HideInInspector]
    private NetworkButtons ButtonsPrevious { get; set; }

    [Networked, HideInInspector]
    public bool CanUse { get; set; }

    [Networked(OnChanged = nameof(OnWeaponsListChanged), OnChangedTargets = OnChangedTargets.All), Capacity(6)]
    public NetworkLinkedList<ItemController> Weapons { get; }

    private NetworkPlayer _player;


    private WeaponContext _context = new WeaponContext();
    
    
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
            itemController.Player = _player;
        
            Weapons.Add(itemController);
        }
    }
    
    public void ToggleInventory()
    {
        Debug.Log(CanUse);
        if (!CanUse)
        {
            return;
        }
        
        IsOpen = !IsOpen;
        
        if (Object.HasInputAuthority)
        {
            inventoryUI.SetActive(IsOpen);
            _player.Camera.CanLook = !IsOpen;

            if (IsOpen)
            {
                Context.Input.RequestCursorRelease();
            }
            else
            {
                Context.Input.RequestCursorLock();
            }
        }
    }
    
    private void SelectSlot(int i)
    {
        if (!CheckSlots(i, out int itemID))
        {
            return;
        }

        if (itemID == EquippedItem.item.itemID)
        {
       //     return;
        }

        if (IsSwitching)
        {
            return;
        }

        
        ItemController itemController = FindItemController(itemID);

        if (itemController == null)
        {
            return;
        }

        SwitchItemControllers(itemController, i);
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

        foreach (ItemController wpn in Weapons)
        {
            if (wpn.Player != null)
            {
                continue;
            }

            wpn.Player = _player;
        }
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
    
    private void SwitchItemControllers(ItemController nextController, int slotID)
    {
        if (EquippedItem != null)
        {
            StartCoroutine(IE_SwitchItems(nextController, slotID));
        }
        else
        {
            nextController.gameObject.SetActive(true);
            nextController.Equip();
        }
        
        if (Object.HasStateAuthority)
        {
            EquippedItemID = nextController.ID;
        }
    }

    public void HideCurrentItem()
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

    private IEnumerator IE_SwitchItems(ItemController nextController, int newSlotID)
    {
        IsSwitching = true;

        if (Object.HasInputAuthority)
        {
            Slot[] selectedSlots = SlotHandler.FindSelectedSlots();

            for (int i = 0; i < selectedSlots.Length; i++)
            {
                selectedSlots[i].DeselectSlot();
            }
        }
        
        EquippedItem.Hide();
        float hideTime = EquippedItem.GetHideTime();
        yield return new WaitForSeconds(hideTime);

        EquippedItem.gameObject.SetActive(false);
        nextController.gameObject.SetActive(true);
        nextController.Equip();
        
        if (Object.HasInputAuthority)
            SlotHandler.Slots[newSlotID].SelectSlot();
        
        float equipTime = nextController.GetEquipTime();
        yield return new WaitForSeconds(equipTime);

        IsSwitching = false;
    }
    
    private ItemController FindItemController(int itemID)
    {
        for (int i = 0; i < Weapons.Count; i++)
        {
            if (itemID == Weapons[i].item.itemID)
            {
                return Weapons[i];
            }
        }

        return null;
    }

    public void ProcessInput(NetworkInputData input)
    {
        if (_player.Movement.CurrentState != PlayerStates.PlayerController)
        {
            return;
        }

        if (_player.Pause.IsOpen)
        {
            return;
        }
        
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
        
        if (pressed.IsSet(PlayerButtons.Slot2))
        {
            SelectSlot(16);
        }
        
        if (pressed.IsSet(PlayerButtons.Slot3))
        {
            SelectSlot(17);
        }
        
        if (pressed.IsSet(PlayerButtons.Slot4))
        {
            SelectSlot(18);
        }
        
        if (pressed.IsSet(PlayerButtons.Slot5))
        {
            SelectSlot(19);
        }

        if (EquippedItem != null)
        {
            _context.Input = input;
            _context.FireDirection = (input.LookForward * input.LookVertical) * Vector3.forward;
            _context.FirePosition = fireTransform.position;

            EquippedItem.ProcessInput(_context);
        }
    }

    public void ThrowItem()
    {
        
    }
}
