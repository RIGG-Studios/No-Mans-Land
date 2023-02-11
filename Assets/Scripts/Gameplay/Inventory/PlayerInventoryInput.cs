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
      //  InputActions.Player.ToggleInventory.performed += ctx => _playerInventory.ToggleInventory();
    }
}
