using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAnimator : MonoBehaviour
{
    private Animator _animator;
    
    
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsSprinting = Animator.StringToHash("IsSprinting");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        bool isWalking = NetworkPlayer.Local.Movement.MovementState == PlayerMovementStates.Moving;
        bool isSprinting = NetworkPlayer.Local.Movement.MovementState == PlayerMovementStates.Sprinting;
        
        _animator.SetBool(IsWalking, isWalking);
        _animator.SetBool(IsSprinting, isSprinting);

    }
}
