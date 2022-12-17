using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class CameraLook : MonoBehaviour
{
    public bool CanLook { get; set; }
    
    [SerializeField] private Transform camAnchor;
    [SerializeField] private float mouseSensitivity;
    
    private float _lookX;
    private float _lookY;
    
    float cameraRotationX = 0;
    float cameraRotationY = 0;
    
    private float _fov;
    private float _defaultFOV;

    private Camera _camera;
    
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        _camera = GetComponentInChildren<Camera>();
        _defaultFOV = _camera.fieldOfView;
        _fov = _defaultFOV;
        CanLook = true;
    }

    private void Start()
    {
        if (_camera.enabled)
        {
            transform.parent = null;
        }
    }

    private void LateUpdate()
    {
        if (!CanLook)
        {
            return;
        }
        
        UpdateCameraLook();
    }

    private void UpdateCameraLook()
    {
        transform.position = camAnchor.position;
        
        cameraRotationX += _lookY * Time.deltaTime * mouseSensitivity;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90, 90);

        cameraRotationY += _lookX * Time.deltaTime * mouseSensitivity;
        transform.rotation = Quaternion.Euler(-cameraRotationX, cameraRotationY, 0);
    }
    
    
    public void UpdateLookDirection(float hor, float ver)
    {
        _lookY = ver;
        _lookX = hor;
    }

    public void SetFOV(float fov, bool defaultFov = false)
    {
        if (defaultFov)
        {
            _fov = _defaultFOV;
            return;
        }

        _fov = fov;
    }
}
