using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class CustomPassManager : MonoBehaviour
{
    private CustomPassVolume _customPass;

    private void Awake()
    {
        _customPass = GetComponent<CustomPassVolume>();
    }

    private void Update()
    {
        if (NetworkPlayer.Local == null)
        {
            return;
        }
        
        _customPass.enabled = !NetworkPlayer.Local.Movement.CameraSubmerged;
    }
}
