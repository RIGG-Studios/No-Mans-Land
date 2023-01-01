using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : ContextBehaviour
{
    public bool IsArmed { get; private set; }

    public Item Item => item;

    [SerializeField] private Item item;
    
    private WeaponAction[] _weaponActions;

    public virtual void ProcessInput(NetworkInputData context)
    {
        bool isBusy = IsBusy();

        for (int i = 0; i < _weaponActions.Length; i++)
        {
            if (i > 0 && !isBusy)
            {
                isBusy |= _weaponActions[i - 1].IsBusy();
            }
            
            _weaponActions[i].ProcessInput(context, isBusy);
        }
    }

    public virtual void OnRender()
    {
        for (int i = 0; i < _weaponActions.Length; i++)
        {
            _weaponActions[i].DoRender();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        
        _weaponActions = GetComponentsInChildren<WeaponAction>(false);

        if (_weaponActions.Length > 0)
        {
            for (int i = 0; i < _weaponActions.Length; i++)
            {
                _weaponActions[i].Init(this, (byte)i);
            }
        }
    }
    
    public bool IsBusy()
    {
        for (int i = 0; i < _weaponActions.Length; i++)
        {
            if (_weaponActions[i].IsBusy())
                return true;
        }

        return false;
    }
}
