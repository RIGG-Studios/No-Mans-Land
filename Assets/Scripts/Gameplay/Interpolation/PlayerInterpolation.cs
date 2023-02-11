using System;
using UnityEngine;


public class PlayerInterpolation : InterpolationManager
{
    [SerializeField] private InterpolationProperties[] interpolationProperties;

    [Serializable]
    private struct InterpolationProperties
    {
        public string id;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public float time;

        public Interpolation interpolation;
        public Easing easing;
        public AnimationCurve curve;
    }

    private int _currentInterpolationIndex = -1;

    private CustomPositionInterpolation GetInterpolationPosition()
    {
        InterpolationProperties props = interpolationProperties[_currentInterpolationIndex];

        if (props.interpolation == Interpolation.EASE)
        {
            switch (props.easing)
            {
                case Easing.BOUNCE_IN:
                    return InterpolationTransitions.EaseInBounce;
                
                case Easing.BOUNCE_OUT:
                    return InterpolationTransitions.EaseOutBounce;
                
                case Easing.SPRING:
                    return InterpolationTransitions.EaseSpringPosition;
                
                case Easing.BACK:
                    return InterpolationTransitions.EaseBackPosition;
                
                case Easing.SINE:
                    return InterpolationTransitions.EaseSinePosition;
                
                case Easing.CUBIC:
                    return InterpolationTransitions.EaseCubicPosition;
                
                case Easing.CIRCLE:
                    return InterpolationTransitions.EaseCirclePosition;
                
                case Easing.ELASTIC:
                    return InterpolationTransitions.EaseElasticPosition;
                
                case Easing.QUARTIC:
                    return InterpolationTransitions.EaseQuarticPosition;
                
                case Easing.QUINTIC:
                    return InterpolationTransitions.EaseQuinticPosition;
                
                case Easing.QUADRATIC:
                    return InterpolationTransitions.EaseQuadraticPosition;

                case Easing.EXPONENTIAL:
                    return InterpolationTransitions.EaseExponentialPosition;
            }
        }
        else
        {
            switch (props.interpolation)
            {
                case Interpolation.SMOOTH:
                    return InterpolationTransitions.SmoothPosition;
                
                case Interpolation.LINEAR:
                    return InterpolationTransitions.LinearPosition;
            }
        }

        return null;
    }
    
    private CustomRotationInterpolation GetInterpolationRotation()
    {
        InterpolationProperties props = interpolationProperties[_currentInterpolationIndex];

        if (props.interpolation == Interpolation.EASE)
        {
            switch (props.easing)
            {
                case Easing.BACK:
                    return InterpolationTransitions.EaseBackRotation;
                
                case Easing.SINE:
                    return InterpolationTransitions.EaseSineRotation;
                
                case Easing.CUBIC:
                    return InterpolationTransitions.EaseCubicRotation;
                
                case Easing.CIRCLE:
                    return InterpolationTransitions.EaseCircleRotation;
                
                case Easing.ELASTIC:
                    return InterpolationTransitions.EaseElasticRotation;
                
                case Easing.QUARTIC:
                    return InterpolationTransitions.EaseQuarticRotation;
                
                case Easing.QUINTIC:
                    return InterpolationTransitions.EaseQuinticRotation;
                
                case Easing.QUADRATIC:
                    return InterpolationTransitions.EaseQuadraticRotation;

                case Easing.EXPONENTIAL:
                    return InterpolationTransitions.EaseExponentialRotation;
            }
        }
        else
        {
            switch (props.interpolation)
            {
                case Interpolation.SMOOTH:
                    return InterpolationTransitions.SmoothRotation;
                
                case Interpolation.LINEAR:
                    return InterpolationTransitions.LinearRotation;
            }
        }

        return null;
    }
    
    

    private void EnterInterpolation(int index)
    {
        if (index == -1)
            return;

        CustomPositionInterpolation positionInterpolation = GetInterpolationPosition();
        CustomRotationInterpolation rotationInterpolation = GetInterpolationRotation();

        InterpolationProperties properties = interpolationProperties[index];
        
        SetPose(properties.position, positionInterpolation, Quaternion.Euler(properties.rotation),
            rotationInterpolation, properties.scale, positionInterpolation, properties.time);
    }

    public void ExitInterpolation()
    {
        if (_currentInterpolationIndex == -1)
            return;
        
        CustomPositionInterpolation positionInterpolation = GetInterpolationPosition();
        CustomRotationInterpolation rotationInterpolation = GetInterpolationRotation();

        InterpolationProperties properties = interpolationProperties[_currentInterpolationIndex];
        
        ResetPose(properties.position, positionInterpolation, rotationInterpolation, properties.scale, 
            positionInterpolation, properties.time);
    }

    private void Update()
    {
        UpdateInterpolation();
    }

    public void SetInterpolation(string interpolationID)
    {
        int newIndex = GetInterpolationIndex(interpolationID);

        if (_currentInterpolationIndex != -1)
        {
            ExitInterpolation();
            _currentInterpolationIndex = -1;
        }
        
        if (_currentInterpolationIndex != newIndex)
        {
            _currentInterpolationIndex = newIndex;
            EnterInterpolation(newIndex);
        }
    }

    public int GetInterpolationIndex(string id)
    {
        for (int i = 0; i < interpolationProperties.Length; i++)
        {
            if (interpolationProperties[i].id == id)
            {
                return i;
            }
        }

        return -1;
    }
}
