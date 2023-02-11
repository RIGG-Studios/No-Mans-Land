using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ExampleAnimator : MonoBehaviour
{
    [SerializeField] private CharacterEquippableItem characterEquippableItem;

    private Animator _animator;

    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsSprinting = Animator.StringToHash("IsSprinting");
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    

    public void Update()
    {
        bool isSprinting = characterEquippableItem.Player.Movement.IsSprinting &&
                           !characterEquippableItem.Player.Movement.IsSwimming;

        Debug.Log(isSprinting);
         bool isWalking = characterEquippableItem.Player.Movement.IsMoving &&
                          !characterEquippableItem.Player.Movement.IsSwimming && !isSprinting;
         
         Debug.Log(isWalking);
         _animator.SetBool(IsWalking, isWalking);
         _animator.SetBool(IsSprinting, isSprinting);
    }
}
