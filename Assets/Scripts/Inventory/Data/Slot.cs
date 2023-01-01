using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image itemIcon;
    [SerializeField] protected Text stackText;

    public int ID { get; set; }
    public bool HasItem { get; set; }
    public IInventory Inventory { get; set; }
    public bool IsHovered { get; set; }
    
    private SlotHandler _slotHandler;

    public delegate void OnSlotReset(Slot slot);

    public OnSlotReset SlotReset;

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

    public void InitItem(Item item, ref ItemListData inventoryItem)
    {
        itemIcon.enabled = true;
        itemIcon.sprite = item.itemIcon;

        if (item.IsStackable)
        {
            stackText.enabled = true;
        }

        InventoryItem = inventoryItem;
        HasItem = true;
    }

    public virtual void Reset()
    {
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

        Slot[] slots = SlotSpawner.AllSlots.ToArray();

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsHovered = false;
    }
}