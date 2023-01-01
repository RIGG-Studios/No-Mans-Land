using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryInput : InputBase
{
    private PlayerInventory _playerInventory;



    public override void Awake()
    {
        base.Awake();
        _playerInventory = GetComponent<PlayerInventory>();
    }

    private void Start()
    {
        InputActions.Player.ToggleInventory.performed += ctx => _playerInventory.ToggleInventory();
        
        InputActions.Player.Slot1.performed += ctx => _playerInventory.SelectSlot(15);
        InputActions.Player.Slot2.performed += ctx => _playerInventory.SelectSlot(16);
        InputActions.Player.Slot3.performed += ctx => _playerInventory.SelectSlot(17);
        InputActions.Player.Slot4.performed += ctx => _playerInventory.SelectSlot(18);
        InputActions.Player.Slot5.performed += ctx => _playerInventory.SelectSlot(19);
    }
}
