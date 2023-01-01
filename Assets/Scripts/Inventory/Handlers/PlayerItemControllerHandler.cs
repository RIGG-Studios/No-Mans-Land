using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerItemControllerHandler : NetworkBehaviour
{
    [SerializeField] private Transform itemHolder;
    
    public List<ItemController> itemControllers = new();


    public void OnItemAdded(ItemListData itemData)
    {
        if (!Object.HasInputAuthority)
        {
            return;
        }
        
        Item item = SceneHandler.Instance.ItemDatabase.FindItem(itemData.ItemID);

        if (!item.isEquippable)
        {
            return;
        }
        
        InitFPItem(item, ref itemData);
    }

    private void InitFPItem(Item item, ref ItemListData itemData)
    {
        ItemController itemController = Instantiate(item.fpPrefab, itemHolder).GetComponent<ItemController>();

        itemController.Init(NetworkPlayer.Local, item, ref itemData);
        itemController.gameObject.SetActive(false);
        
        itemControllers.Add(itemController);
    }
}
