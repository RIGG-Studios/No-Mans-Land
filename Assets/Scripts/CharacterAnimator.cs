using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CharacterAnimator : SimulationBehaviour
{
    [SerializeField] private NetworkPlayer player;
    
    private Animator _animator;
    private static readonly int IsSprinting = Animator.StringToHash("IsSprinting");
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int InputMagnitude = Animator.StringToHash("InputMagnitude");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public override void Render()
    {      
        bool isSprinting = player.Movement.IsSprinting &&
                                !player.Movement.IsSwimming;
        
        float vertical = player.Movement.Vertical;
        float horizontal = player.Movement.Horizontal;
        float magnitude = new Vector2(horizontal, vertical).magnitude;
        
        Debug.Log(isSprinting);
        
        _animator.SetBool(IsSprinting, isSprinting);
        _animator.SetFloat(Vertical, vertical);
        _animator.SetFloat(Horizontal, horizontal);
        _animator.SetFloat(InputMagnitude, magnitude);
        
    }
}
