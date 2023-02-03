using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class ChestInventory : Inventory, IInteractable
{
    [Networked] 
    public NetworkBool IsOpen { get; set; } = false;

    [SerializeField] private GameObject chestUI;
    
    private bool _canOpen;
    
    public string LookAtID { get; set; }
    public string ID => "Chest";
    public PlayerButtons ExitKey => PlayerButtons.ToggleInventory;

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
        if (IsOpen)
        {
            interactionData = default;
            return false;
        }
        
        if(Object.HasStateAuthority)
        {
            IsOpen = true;
            Object.AssignInputAuthority(player.Object.InputAuthority);
        }

        chestUI.SetActive(true);


        interactionData = new ButtonInteractionData()
        {
            StopMovement = true,
            EnableCursor = true,
            OpenInventory = true,
            StopCameraLook = true
        };
        
        return true;
    }

    public void StopButtonInteract(NetworkPlayer player, out ButtonInteractionData interactionData)
    {
        chestUI.SetActive(false);

        if(Object.HasStateAuthority)
        {
            IsOpen = false;
            Object.AssignInputAuthority(default);
        }
        
        interactionData = new ButtonInteractionData()
        {
            EnableMovement = false,
            DisableCursor = true,
            HideInventory = true,
            EnableCameraLook = true
        };
    }
    
}
