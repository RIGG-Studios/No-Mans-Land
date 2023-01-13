using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCamera : SceneComponent
{
    public Camera Camera => sceneCamera;
    
    [SerializeField] private Camera sceneCamera;

    [SerializeField] private Easing positionEasing;
    [SerializeField] private Easing rotationEasing;

    private Vector3 _startPos;
    private Quaternion _startRot;

    private CustomPositionInterpolation _positionInterpolation;
    private CustomRotationInterpolation _rotationInterpolation;
    private Transform _target;

    private void Awake()
    {
        _startPos = transform.position;
        _startRot = transform.rotation;
    }

    protected override void OnInit()
    {
        base.OnInit();
        
        Scene.Context.Camera = this;
    }

    public void Enable()
    {
        Camera.gameObject.SetActive(true);
    }

    public void Disable()
    {
        Camera.gameObject.SetActive(false);

        transform.position = _startPos;
        transform.rotation = _startRot;
        _target = null;
    }

    private void Update()
    {
        if (_target == null)
        {
            return;
        }
        
        transform.position = _positionInterpolation(transform.position, _target.position, 6.0f * Time.deltaTime);
        transform.rotation = _rotationInterpolation(transform.rotation, _target.rotation, 6.0f * Time.deltaTime);
    }

    public void SmoothLerp(Transform target, float speed)
    {
         _positionInterpolation =
            InterpolationTransitions.GetInterpolationPosition(Interpolation.EASE, positionEasing);

         _rotationInterpolation =
            InterpolationTransitions.GetInterpolationRotation(Interpolation.EASE, rotationEasing);

         _target = target;
    }
}
