using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int ID { get; set; }
    public bool HasItem { get; set; }
    public IInventory inventory { get; set; }
    public bool IsHovered { get; set; }
    public bool IsSelected { get; private set; }
    
    
    [SerializeField] protected Image itemIcon;
    [SerializeField] protected Text stackText;

    public delegate void OnSlotReset(Slot slot);
    public OnSlotReset SlotReset;
    
    
    private SlotHandler _slotHandler;
    private Animator _animator;

    
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
        this.inventory = inventory;
    }
    
    [HideInInspector]
    public ItemListData InventoryItem;

    public virtual void InitItem(Item item, ItemListData inventoryItem)
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
        _animator.SetTrigger("Select");
        IsSelected = true;
    }

    public void DeselectSlot()
    {
        _animator.SetTrigger("Hide");
        IsSelected = false;
    }

    public virtual void Reset()
    {
        if (HasItem)
        {
            SlotReset?.Invoke(this);

            if (IsSelected)
            {
                DeselectSlot();
            }
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
            else if (closestDist >= 100f)
            {
                inventory.ThrowItem(this);
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
        inventory.OnSlotHovered(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsHovered = false;
        inventory.OnSlotUnHovered(this);
    }
}
