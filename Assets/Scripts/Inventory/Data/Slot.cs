using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int ID { get; set; }
    public bool HasItem { get; set; }
    public IInventory Inventory { get; set; }
    public bool IsHovered { get; set; }
    
    
    [SerializeField] protected Image itemIcon;
    [SerializeField] protected Text stackText;

    private SlotHandler _slotHandler;
    private Animator _animator;
    
    public delegate void OnSlotReset(Slot slot);
    public OnSlotReset SlotReset;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void InitSlot(IInventory inventory, SlotHandler slotHandler, int id)
    {
        ID = id;

        itemIcon.enabled = false;
        stackText.enabled = false;
        _slotHandler = slotHandler;
        Inventory = inventory;
    }
    
    [HideInInspector]
    public ItemListData InventoryItem;

    public virtual void InitItem(Item item, ref ItemListData inventoryItem)
    {
        itemIcon.enabled = true;
        itemIcon.sprite = item.itemIcon;

        InventoryItem = inventoryItem;
        if (item.IsStackable)
        {
            UpdateItemStackText(inventoryItem.Stack);
            stackText.enabled = true;
        }
        
        HasItem = true;
    }

    public void SelectSlot()
    {
    }

    public virtual void Reset()
    {
        if (HasItem)
        {
            SlotReset?.Invoke(this);
        }

        HasItem = false;

        itemIcon.enabled = false;
        stackText.enabled = false;
        InventoryItem = default;
        
        SlotReset?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        itemIcon.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_slotHandler == null || !HasItem) return;

        Slot[] slots = FindObjectsOfType<Slot>();

        float closestDist = -1f;
        Slot closestSlot = null;

        for (int i = 0; i < slots.Length; i++)
        {
            float dist = Vector2.Distance(itemIcon.transform.position, slots[i].transform.position);
            
            if (closestSlot == null || dist < closestDist)
            {
                closestSlot = slots[i];
                closestDist = dist;
            }
        }

        if (closestSlot != null)
        {
            if (closestDist <= 50f)
            {
                bool toFirstSlot = Keyboard.current.leftCtrlKey.isPressed;
                _slotHandler.MoveItemInSlot(this, closestSlot, toFirstSlot);
                itemIcon.transform.localPosition = Vector3.zero;
            }
            else
            {
                itemIcon.transform.localPosition = Vector3.zero;
            }
        }
        else
        {
            itemIcon.transform.localPosition = Vector3.zero;
        }
    }

    public void UpdateItemStackText(int stack)
    {
        InventoryItem.Stack = stack;
        stackText.text = $"x{stack}";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsHovered = true;
        Inventory.OnSlotHovered(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsHovered = false;
        Inventory.OnSlotUnHovered(this);
    }
}
