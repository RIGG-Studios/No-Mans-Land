using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerItemControllerHandler : ContextBehaviour
{
    [SerializeField] private Transform itemHolder;
    
    public List<ItemController> itemControllers = new();


    public void OnItemAdded(ItemListData itemData)
    {
        if (!Object.HasInputAuthority)
        {
            return;
        }
        
        Item item = Context.ItemDatabase.FindItem(itemData.ItemID);

        if (!item.isEquippable)
        {
            return;
        }

        InitFPItem(item);
    }

    private void InitFPItem(Item item)
    {
        ItemController itemController = Instantiate(item.fpPrefab, itemHolder).GetComponent<ItemController>();

        itemController.Init(NetworkPlayer.Local, item);
        itemController.gameObject.SetActive(false);
        
        itemControllers.Add(itemController);
    }
}
