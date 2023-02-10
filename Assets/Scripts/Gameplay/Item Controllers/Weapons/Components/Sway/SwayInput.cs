using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayInput : InputBase
{
    private BaseSway _baseSway;


    public override void Awake()
    {
        base.Awake();

        _baseSway = GetComponent<BaseSway>();
    }


    public void Update()
    {
        Vector2 mousePos = InputActions.Player.Look.ReadValue<Vector2>();

        if (NetworkPlayer.Local == null)
        {
            return;
        }
        
        if (NetworkPlayer.Local.Inventory.IsOpen)
        {
            return;
        }
        
        _baseSway.UpdateInput(mousePos);
    }
}
