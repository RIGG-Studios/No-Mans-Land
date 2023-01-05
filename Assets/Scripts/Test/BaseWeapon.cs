using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct WeaponAnimationData
{
    public string name;
    public AnimationClip clip;

    public float ClipLength => clip.length;
}

public class BaseWeapon : ItemController
{
    public WeaponAnimationData equipAnimationData;
    public WeaponAnimationData hideAnimationData;

    protected Animator Animator;

    protected virtual void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
    }

    public override void Equip() => StartCoroutine(IE_Equip());
    public override void Hide() => StartCoroutine(IE_Hide());
    
    
    public override float GetEquipTime() => equipAnimationData.ClipLength;
    public override float GetHideTime() => hideAnimationData.ClipLength;
    
    
    private IEnumerator IE_Equip()
    {
        Animator.SetTrigger("Equip");
        
        yield return new WaitForSeconds(equipAnimationData.ClipLength);
    }

    private IEnumerator IE_Hide()
    {
        Animator.SetTrigger("Hide");
        
        yield return new WaitForSeconds(hideAnimationData.ClipLength);
    }
}
