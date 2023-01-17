using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInput : SceneComponent
{
    public bool IsLocked => Cursor.lockState == CursorLockMode.Locked;
    
    
    public void RequestCursorLock()
    {
        SetLockedState(true);
    }

    public void RequestCursorRelease()
    {
        SetLockedState(false);

    }
    
    
    private void SetLockedState(bool value)
    {
        Cursor.lockState = value == true ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !value;
    }

}
