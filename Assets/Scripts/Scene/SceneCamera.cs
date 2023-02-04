using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCamera : SceneComponent
{
    public Camera Camera => _activeCamera;
    
    [SerializeField] private Camera depolyCamera;
    [SerializeField] private Camera sceneLookCamera;

    private Camera _activeCamera => sceneLookCamera;
    
    public enum CameraTypes
    {
        Deploy,
        Scene
    }

    
    private Vector3 _startPos;
    private Quaternion _startRot;
    private Transform _target;

    private void Awake()
    {
        _startPos = depolyCamera.transform.position;
        _startRot = depolyCamera.transform.rotation;
    }

    private void Start()
    {
        depolyCamera.gameObject.SetActive(false);
        sceneLookCamera.gameObject.SetActive(true);
    }

    protected override void OnInit()
    {
        base.OnInit();
        
        Scene.Context.Camera = this;
    }

    public void Enable(CameraTypes type)
    {
        switch (type)
        {
            case CameraTypes.Deploy:
                depolyCamera.gameObject.SetActive(true);
                return;
            
            case CameraTypes.Scene:
                sceneLookCamera.gameObject.SetActive(true);
                return;
        }
    }

    public void Disable(CameraTypes type)
    {
        switch (type)
        {
            case CameraTypes.Deploy:
                depolyCamera.gameObject.SetActive(false);
                return;
            
            case CameraTypes.Scene:
                sceneLookCamera.gameObject.SetActive(false);
                return;
        }

        depolyCamera.transform.position = _startPos;
        depolyCamera.transform.rotation = _startRot;
        _target = null;
    }

    private void Update()
    {
        if (_target == null)
        {
            return;
        }
        
        depolyCamera.transform.position = Vector3.Lerp(depolyCamera.transform.position, _target.position, 2.0f * Time.deltaTime);
        depolyCamera.transform.rotation = Quaternion.Lerp(depolyCamera.transform.rotation, _target.rotation, 2.0f * Time.deltaTime);
    }

    public void SmoothLerp(Transform target, float speed)
    {
        _target = target;
    }
}
