using System;
using Fusion;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class ProceduralAim : WeaponComponent, IAimer
{
    [SerializeField] private Transform aimTransform;
    [SerializeField] private float aimSpeed;
    [SerializeField] private float aimFOV;
    [SerializeField] private Vector3 aimPos;


    [Networked]
    public bool IsAiming { get; private set; }
    
    public IAimer.AimTypes AimType { get; private set; }
    public event Action<bool> onAim;

    private Vector3 _defaultAimPos;
    private Quaternion _defaultAimRot;

    public override void OnEnable()
    {
        base.OnEnable();
        
        Weapon.SetAimer(this);
    }

    public override void ProcessInput(WeaponContext context, ref ItemDesires desires)
    {
        bool aiming = context.Input.IsAiming;

        desires.Aim = aiming;
    }

    public override void FixedUpdateNetwork(WeaponContext context, ItemDesires desires)
    {
        IsAiming = desires.Aim;

        if (Object.HasInputAuthority)
        {
            Weapon.Player.Camera.SetFOV(aimFOV, !IsAiming);
        }
        
        Vector3 pos = !IsAiming ? _defaultAimPos : aimPos;
        Quaternion rot = !IsAiming ? _defaultAimRot : quaternion.identity;

        aimTransform.localPosition = Vector3.Lerp(aimTransform.localPosition, pos, Runner.DeltaTime * aimSpeed);
        aimTransform.localRotation = Quaternion.Lerp(aimTransform.localRotation, rot, Runner.DeltaTime * aimSpeed);
    }
    
    
    protected override void Awake()
    {
        base.Awake();

        _defaultAimPos = aimTransform.localPosition;
        _defaultAimRot = aimTransform.localRotation;
    }
    
    public void ToggleAim()
    {
    //    IsAiming = !IsAiming;
    //    onAim?.Invoke(IsAiming);
    }
}
