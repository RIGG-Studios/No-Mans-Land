using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class ChestInventory : NetworkInventory, IInteractable
{
    [Networked] 
    public NetworkBool IsOpen { get; set; } = false;

    [SerializeField] private GameObject chestUI;
    
    private bool _canOpen;
    
    public string LookAtID { get; set; }
    public string ID => "Chest";

    private void Start()
    {
        chestUI.SetActive(false);
    }

    public void LookAtInteract()
    {
        LookAtID = IsOpen ? "CHEST IN USE" : "[F] INTERACT";
    }

    public void StopLookAtInteract()
    {
        LookAtID = string.Empty;
    }
    
    public bool ButtonInteract(NetworkPlayer player, out ButtonInteractionData interactionData)
    {
        bool success = true;
        
        if (IsOpen)
        {
            success = false;
        }
        
        RefreshInventory();
        RPC_RequestChestStatus(true);
        chestUI.SetActive(true);

        interactionData = new ButtonInteractionData()
        {
            StopMovement = true,
            EnableCursor = true,
            OpenInventory = true,
            StopCameraLook = true
        };
        
        return success;
    }

    public void StopButtonInteract(out ButtonInteractionData interactionData)
    {
        chestUI.SetActive(false);
        RPC_RequestChestStatus(false);
        RPC_RequestUpdateInventory(Items.ToArray());
        
        interactionData = new ButtonInteractionData()
        {
            EnableMovement = false,
            DisableCursor = true,
            HideInventory = true,
            EnableCameraLook = true
        };
    }

    protected override void RequestUpdateItems(int itemID, int newSlotID)
    {
        RPC_RequestUpdateItems(itemID, newSlotID);
    }

    protected override void RequestAddItem(ItemListData itemListData)
    {
        RPC_RequestAddItem(itemListData);
    }

    protected override void RequestRemoveItem(ItemListData itemListData)
    {
        RPC_RequestRemoveItem(itemListData);
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestRemoveItem(ItemListData itemListData)
    {
        if (!Object.HasStateAuthority)
        {
            return;
        }

        
        Items.Remove(itemListData);
    }

    
    
    [Rpc(sources : RpcSources.All, targets: RpcTargets.StateAuthority)]  
    private void RPC_RequestChestStatus(bool open)
    {
        if (!Object.HasStateAuthority)
        {
            return;
        }

        RPC_ExecuteChestStatus(open);
    }
    
    
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ExecuteChestStatus(bool isOpen)
    {
        IsOpen = isOpen;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestUpdateInventory(ItemListData[] newItems)
    {
        if (!Object.HasStateAuthority)
        {
            return;
        }

        RPC_ExecuteUpdateInventory(newItems);
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ExecuteUpdateInventory(ItemListData[] newItems)
    {
        UpdateInventory(newItems);
    }
    

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestAddItem(ItemListData itemListData)
    {
        if (!Object.HasStateAuthority)
        {
            return;
        }

        
        Items.Add(itemListData);
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestUpdateItems(int itemID, int newSlotID)
    {
        if (!Object.HasStateAuthority)
        {
            return;
        }

        ExecuteUpdateItems(itemID, newSlotID);
    }  
    
    
    //method to update the local inventory to the server inventory
    private void UpdateInventory(ItemListData[] newItems)
    {
        for (int i = 0; i < newItems.Length; i++)
        {
            Slot[] slots = SlotHandler.Slots;
            
            for (int z = 0; z < slots.Length; z++)
            {
                slots[z].Reset();
                if (newItems[i].SlotID == slots[z].ID)
                {
                    Item item = SceneHandler.Instance.ItemDatabase.FindItem(newItems[i].ItemID);
                    
                    slots[z].InitItem(item, ref newItems[i]);
                }
            }
        }
    }
}
