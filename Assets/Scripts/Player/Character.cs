using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Character : ContextBehaviour
{
    [SerializeField] private Transform weaponsContainer;
    
    private List<CharacterEquippableItem> _tpItems = new();

    private CharacterEquippableItem _currentItem;
    private int _currentTpID;
    
    private void Start()
    {
        Item[] items = Context.ItemDatabase.GetEquippableItems();

        for (int i = 0; i < items.Length; i++)
        {
            SetupItemModel(items[i]);
        }
    }

    private void SetupItemModel(Item item)
    {
        CharacterEquippableItem itemController =
            Instantiate(item.tpPrefab, weaponsContainer).GetComponent<CharacterEquippableItem>();
        
        itemController.Init(item.itemID);
        itemController.gameObject.SetActive(false);

        _tpItems.Add(itemController);
    }

    public void OnItemsChanged(int newID)
    { 
        if (Object.HasInputAuthority)
        {
            return;
        }
        
        CharacterEquippableItem current = FindEquippableItem(_currentTpID);

        if (current != null)
        {
            current.gameObject.SetActive(false);
        }

        CharacterEquippableItem next = FindEquippableItem(newID);

        if (next != null)
        {
            _currentItem = next;
            next.gameObject.SetActive(true);
        }

        _currentTpID = newID;
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData input))
        {
            return;
        }

        if (input.CurrentWeaponID != _currentTpID)
        {
            OnItemsChanged(input.CurrentWeaponID);
        }

        _currentTpID = input.CurrentWeaponID;
    }

    public void OnAttack()
    {
        if (_currentItem == null)
        {
            return;
        }
        
        _currentItem.OnAttack();
    }

    private CharacterEquippableItem FindEquippableItem(int id)
    {
        CharacterEquippableItem item = null;

        for (int i = 0; i < _tpItems.Count; i++)
        {
            if (id != _tpItems[i].ItemID)
            {
                continue;
            }

            item = _tpItems[i];
        }

        return item;
    }
}
