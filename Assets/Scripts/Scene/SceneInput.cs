using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInput : SceneComponent
{
    public bool IsLocked => Cursor.lockState == CursorLockMode.Locked;

    private static int _cursorLockRequests;
    
    
    public void RequestCursorLock()
    {
        _cursorLockRequests++;

        if (_cursorLockRequests == 1)
        {
            SetLockedState(true);
        }
    }

    public void RequestCursorRelease()
    {
        _cursorLockRequests--;

        if (_cursorLockRequests == 0)
        {
            SetLockedState(false);
        }
    }
    
    
    private void SetLockedState(bool value)
    {
        Cursor.lockState = value == true ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !value;
    }

}
