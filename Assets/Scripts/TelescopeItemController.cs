using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class TelescopeItemController : BaseWeapon
{
    [SerializeField] private GameObject render;
    [SerializeField, Range(10, 60)] private float lookFOV;
    
    [Networked] 
    public NetworkBool IsLooking { get; set; }


    
    public override void ProcessInput(WeaponContext context)
    {
        bool aimIn = context.Input.IsAiming && !Player.Movement.IsSwimming;

        if (!IsLooking && aimIn)
        {
            if (Object.HasInputAuthority)
            {
                Animator.SetTrigger("Look");
            }
            
            IsLooking = true;

        }
        else if(IsLooking && !aimIn)
        {
            IsLooking = false;
        }
    }

    private void Update()
    {
        if (!Object.HasInputAuthority)
        {
            return;
        }
        
        Player.Camera.SetFOV(lookFOV, !IsLooking);

        if (IsLooking)
        {
            Context.PostProcessing.EnablePostProcessing(ScenePostProcessing.PostProcessingTypes.Telescope);
            render.SetActive(false);
        }
        else
        {
            Context.PostProcessing.DisablePostProcessing(ScenePostProcessing.PostProcessingTypes.Telescope);
            render.SetActive(true);
        }
    }
}
