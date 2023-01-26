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
        _startPos = sceneCamera.transform.position;
        _startRot = sceneCamera.transform.rotation;
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

        sceneCamera.transform.position = _startPos;
        sceneCamera.transform.rotation = _startRot;
        _target = null;
    }

    private void Update()
    {
        if (_target == null)
        {
            return;
        }
        
        sceneCamera.transform.position = Vector3.Lerp(sceneCamera.transform.position, _target.position, 2.0f * Time.deltaTime);
        sceneCamera.transform.rotation = Quaternion.Lerp(sceneCamera.transform.rotation, _target.rotation, 2.0f * Time.deltaTime);
    }

    public void SmoothLerp(Transform target, float speed)
    {
        _target = target;
    }
}
