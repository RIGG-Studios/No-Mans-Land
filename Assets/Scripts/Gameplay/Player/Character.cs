using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Character : ContextBehaviour
{
    [SerializeField] private Transform weaponsContainer;
    [SerializeField] private Transform modelRoot;
    
    private List<CharacterEquippableItem> _tpItems = new();

    private CharacterEquippableItem _currentItem;
    private NetworkPlayer _player;
    private int _currentTpID;

    public CharacterEquippableItem CurrentItem => _currentItem;

    private Rigidbody[] _rigidbodies;
    
    private void Start()
    {
        _player = GetComponent<NetworkPlayer>();
        Item[] items = Context.ItemDatabase.GetEquippableItems();

        for (int i = 0; i < items.Length; i++)
        {
            SetupItemModel(items[i]);
        }
    }

    public override void Spawned()
    {
        _rigidbodies = modelRoot.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in _rigidbodies)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }
    
    private void SetupItemModel(Item item)
    {
        CharacterEquippableItem itemController =
            Instantiate(item.tpPrefab, weaponsContainer).GetComponent<CharacterEquippableItem>();
        
        itemController.Init(item.itemID, _player);
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

    public void EnableRagdoll()
    {
        foreach (Rigidbody rb in _rigidbodies)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        _currentItem.gameObject.SetActive(false);
    }

    public void DropCurrentItem()
    {
        if (_currentItem == null)
        {
            return;
        }
        
        
        Item item = Context.ItemDatabase.FindItem(_currentItem.ItemID);
        ItemPickup pickup = Runner.Spawn(item.pickupPrefab, weaponsContainer.position, weaponsContainer.rotation)
            .GetComponent<ItemPickup>();
        
        pickup.Init(_currentItem.ItemID, 1);
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
