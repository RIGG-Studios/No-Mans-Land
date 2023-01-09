using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAnimator : MonoBehaviour
{
    [SerializeField] private Transform movementTransform;
    [SerializeField] private float horizontalMovementAngle;
    
    private Animator _animator;
    
    
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsSprinting = Animator.StringToHash("IsSprinting");
    private static readonly int IsAiming = Animator.StringToHash("IsAiming");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateAnimator();
        
        UpdateTransform();
    }

    private void UpdateAnimator()
    {
        ItemControllerState currentItemState = NetworkPlayer.Local.Inventory.GetEquippedItemState();

        bool isAiming = currentItemState.IsAiming;
        bool isSprinting = NetworkPlayer.Local.Movement.IsSprinting && !isAiming;
        bool isWalking = NetworkPlayer.Local.Movement.IsMoving && !isSprinting && !isAiming;
        
        _animator.SetBool(IsAiming, isAiming);
        _animator.SetBool(IsWalking, isWalking);
        _animator.SetBool(IsSprinting, isSprinting);
    }

    private void UpdateTransform()
    {
        float angle = NetworkPlayer.Local.Movement.Horizontal * horizontalMovementAngle;
        
        movementTransform.localRotation = Quaternion.Slerp(movementTransform.localRotation, 
            Quaternion.Euler(0.0f, 0.0f, angle), Time.deltaTime * 5f);
    }
}
