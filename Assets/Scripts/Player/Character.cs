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
    private NetworkPlayer _player;
    private int _currentTpID;
    
    private void Start()
    {
        _player = GetComponent<NetworkPlayer>();
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

        int newItemID = _player.Inventory.EquippedItem.item.itemID;
        
        CharacterEquippableItem next = FindEquippableItem(newItemID);

        if (next != null)
        {
            _currentItem = next;
            next.gameObject.SetActive(true);
        }

        _currentTpID =  newItemID;
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
