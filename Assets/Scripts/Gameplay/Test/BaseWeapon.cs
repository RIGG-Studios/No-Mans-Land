using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
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
    
    protected bool IsEquipping { get; set; }
    protected bool IsHiding { get; set; }

    protected Animator Animator;

    private Renderer[] _renderers;

    protected override void Awake()
    {
        base.Awake();
        Animator = GetComponentInChildren<Animator>();
        _renderers = GetComponentsInChildren<Renderer>();
    }

    public override T GetService<T>()
    {
        return null;
    }

    public override void Equip()
    {
        base.Equip();

        if (Object.HasInputAuthority)
        {
            StartCoroutine(IE_Equip());
        }
    }

    public override void Hide()
    {
        base.Hide();

        if (Object.HasInputAuthority)
        {
            StartCoroutine(IE_Hide());
        }
    }


    public override float GetEquipTime() => equipAnimationData.ClipLength;
    public override float GetHideTime() => hideAnimationData.ClipLength;
    
    
    private IEnumerator IE_Equip()
    {
        IsEquipping = true;
        Animator.SetTrigger("Equip");
        
        yield return new WaitForSeconds(equipAnimationData.ClipLength);
        IsEquipping = false;
        
        EnableRenders();
    }

    private IEnumerator IE_Hide()
    {
        IsHiding = true;
        Animator.SetTrigger("Hide");
        
        yield return new WaitForSeconds(hideAnimationData.ClipLength);
        IsHiding = false;
        
        DisableRenders();
    }

    private void DisableRenders()
    {
        if (_renderers.Length <= 0)
        {
            return;
        }

        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].enabled = false;
        }
    }

    private void EnableRenders()
    {
        if (_renderers.Length <= 0)
        {
            return;
        }
        
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].enabled = true;
        }
    }
}
