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
        
        InputActions.Player.Slot1.performed += ctx => _playerInventory.SelectSlot(0);
        InputActions.Player.Slot2.performed += ctx => _playerInventory.SelectSlot(1);
        InputActions.Player.Slot3.performed += ctx => _playerInventory.SelectSlot(2);
        InputActions.Player.Slot4.performed += ctx => _playerInventory.SelectSlot(3);
        InputActions.Player.Slot5.performed += ctx => _playerInventory.SelectSlot(4);
        InputActions.Player.Slot6.performed += ctx => _playerInventory.SelectSlot(5);
    }
}
