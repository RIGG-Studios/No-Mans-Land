using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBase : MonoBehaviour
{
    
    protected InputActions InputActions;

    public virtual void Awake()
    {
        InputActions = new InputActions();
    }

    public virtual void OnEnable()
    {
        InputActions.Enable();
    }

    public virtual void OnDestroy()
    {
        InputActions.Disable();
    }

    public virtual void OnDisable()
    {
        InputActions.Disable();
    }

    public void DisableInput() => InputActions.Disable();

    public void EnableInput() => InputActions.Enable();
}
