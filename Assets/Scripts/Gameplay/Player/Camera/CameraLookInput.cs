using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookInput : InputBase
{
    private CameraLook _cameraLook;


    private void Start()
    {
        _cameraLook = GetComponent<CameraLook>();
    }

    private void Update()
    {
        Vector2 lookDir  = InputActions.Player.Look.ReadValue<Vector2>();
        
        _cameraLook.UpdateLookDirection(lookDir.x, lookDir.y);
    }
}
