using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpolationManager : MonoBehaviour, IInterpolationManager
{
    [SerializeField] private Transform interpolationTransform;

    private Vector3 _idlePos;
    private Vector3 _sourcePos;
    private Vector3 _targetPos;
    
    private Vector3 _idleScale;
    private Vector3 _sourceScale;
    private Vector3 _targetScale;

    private Quaternion _idleRot;
    private Quaternion _sourceRot;
    private Quaternion _targetRot;
    
    private CustomPositionInterpolation _positionInterpolation;
    private CustomPositionInterpolation _scaleInterpolation;
    private CustomRotationInterpolation _rotationInterpolation;

    private float _inverseTransitionDuration;
    private float _interpolationDur;

    private void OnDisable()
    {
        _sourcePos = _targetPos;
        _sourceRot = _targetRot;
        _sourceScale = _targetScale;

        interpolationTransform.localPosition = _targetPos;
        interpolationTransform.localRotation = _targetRot;
        interpolationTransform.localScale = _targetScale;

        _positionInterpolation = null;
        _positionInterpolation = null;
        _interpolationDur = 1f;
    }

    private void Start()
    {
        _idlePos = interpolationTransform.localPosition;
        _idleRot = interpolationTransform.localRotation;
    }

    public void SetPose(Vector3 position, CustomPositionInterpolation posInterpolation, Quaternion rotation,
        CustomRotationInterpolation rotInterpolation, Vector3 scale, CustomPositionInterpolation scaleInterpolation, float duration)
    {
        _sourcePos = interpolationTransform.localPosition;
        _sourceRot = interpolationTransform.localRotation;
        _sourceScale = interpolationTransform.localScale;

        _targetPos = position;
        _targetRot = rotation;
        _targetScale = scale;
        
        _positionInterpolation = position == Vector3.zero ? null : posInterpolation;
        _rotationInterpolation =  rotation == Quaternion.identity ? null : rotInterpolation;
        _scaleInterpolation =  scale == Vector3.zero ? null :scaleInterpolation;

        _interpolationDur = 0f;

        //get inverse duration 
        if (duration < 0.001f)
        {
            _inverseTransitionDuration = 0f;
        }
        else
        {
            _inverseTransitionDuration = 1f / duration;
        }
    }

    public void ResetPose(Vector3 position, CustomPositionInterpolation posInterpolation,
        CustomRotationInterpolation rotInterpolation, Vector3 scale, CustomPositionInterpolation scaleInterpolation, float duration)
    {
        SetPose(_idlePos, _positionInterpolation, _idleRot, _rotationInterpolation, _idleScale, scaleInterpolation, duration);
    }
    
    public void UpdateInterpolation()
    {
        if(_interpolationDur < 1f)
        {
            //increase time multiplied by the inverse time
            if (_inverseTransitionDuration != 0f)
            {
                _interpolationDur += Time.smoothDeltaTime * _inverseTransitionDuration;
            }
            else
            {
                _interpolationDur = 1f;
            }

            if (_interpolationDur >= 1f)
            {
                _interpolationDur = 1f;
                if(_targetPos != Vector3.zero)
                    interpolationTransform.localPosition = _targetPos;
                
                interpolationTransform.localRotation = _targetRot;

                if (_targetScale != Vector3.zero)
                    interpolationTransform.localScale = _targetScale;
            }
            else
            {
                if (_positionInterpolation != null)
                {
                    interpolationTransform.localPosition =
                        _positionInterpolation(_sourcePos, _targetPos, _interpolationDur);
                }

                if (_scaleInterpolation != null)
                {
                    interpolationTransform.localScale =
                        _scaleInterpolation(_sourceScale, _targetScale, _interpolationDur);
                }

                if (_rotationInterpolation != null)
                {
                    interpolationTransform.localRotation =
                        _rotationInterpolation(_sourceRot, _targetRot, _interpolationDur);
                }
            }
        }
    }
    
}
