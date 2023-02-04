

using System;
using UnityEngine.Events;

public interface IAimer 
{
    public enum AimTypes
    {
        Press,
        Hold
    }
    
    bool IsAiming { get; }
    AimTypes AimType { get; }

    event Action<bool> onAim;

    void ToggleAim();
}
