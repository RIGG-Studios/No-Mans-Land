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
        bool isWalking = NetworkPlayer.Local.Movement.CurrentStateType == State.StateTypes.Moving &&
                         NetworkPlayer.Local.Movement.IsMoving;
        
        bool isSprinting = NetworkPlayer.Local.Movement.CurrentStateType == State.StateTypes.Sprinting;
        
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
