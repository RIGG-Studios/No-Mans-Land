using UnityEngine;


public struct InterpolationData
{
    public bool IsValid;
    public bool Return;
    public Vector3 TargetPos;
    public Quaternion TargetRot;
}

public interface IInterpolationManager
{
    void SetPose(Vector3 position, CustomPositionInterpolation posInterpolation, Quaternion rotation,
        CustomRotationInterpolation rotInterpolation, Vector3 scale, CustomPositionInterpolation scaleInterpolation, float duration);
    
    void ResetPose(Vector3 position, CustomPositionInterpolation posInterpolation, 
        CustomRotationInterpolation rotInterpolation, Vector3 scale, CustomPositionInterpolation scaleInterpolation, float duration);
}

public delegate Vector3 CustomPositionInterpolation(Vector3 from, Vector3 to, float lerp);
public delegate Quaternion CustomRotationInterpolation(Quaternion from, Quaternion to, float lerp);