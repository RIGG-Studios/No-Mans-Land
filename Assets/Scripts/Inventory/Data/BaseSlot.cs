using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BaseSlot : MonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IEndDragHandler
{
    [SerializeField] protected Image slotIcon;
    [SerializeField] protected Text stackText;
    
    public int SlotID { get; private set; }
    public bool HasItem { get;  set; }

    protected SlotHandler SlotHandler;
    protected IInventory Inventory;
    
    public void OnDrag(PointerEventData eventData) => OnDragStart(eventData);
    public void OnPointerEnter(PointerEventData eventData) => OnPointerEnter();
    public void OnPointerExit(PointerEventData eventData) => OnPointerExit();
    public void OnEndDrag(PointerEventData eventData) => OnDragEnd(eventData);

    public virtual void Init(int slotID, SlotHandler slotHandler, IInventory inventory)
    {
        SlotID = slotID;

        slotIcon.enabled = false;
        stackText.enabled = false;

        SlotHandler = slotHandler;
        Inventory = inventory;
        HasItem = false;
    }

    protected virtual void OnDragStart(PointerEventData eventData)
    {
        if (!HasItem)
        {
            return;
        }
        
        slotIcon.transform.position = Input.mousePosition;
    }

    protected virtual void OnDragEnd(PointerEventData eventData)
    {
        if (SlotHandler == null || !HasItem)
        {
            return;
        }

        Slot[] slots = SlotSpawner.AllSlots.ToArray();

        float closestDist = -1f;
        Slot closestSlot = null;

        for (int i = 0; i < slots.Length; i++)
        {
            float dist = Vector2.Distance(slotIcon.transform.position, slots[i].transform.position);
            
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
          //      SlotHandler.MoveItemInSlot(this, closestSlot, toFirstSlot);
                
                slotIcon.transform.localPosition = Vector3.zero;
            }
            else
            {
                slotIcon.transform.localPosition = Vector3.zero;
            }
        }
        else
        {
            slotIcon.transform.localPosition = Vector3.zero;
        }
    }
    
    
    public virtual void Reset() { }
    
    public virtual void InitLocalItem(LocalItemData localItemData) { }
    
    public virtual void InitNetworkItem(Item item, ref ItemListData itemListData) { }
    protected virtual void OnPointerEnter() { }
    protected virtual void OnPointerExit() { }

}
