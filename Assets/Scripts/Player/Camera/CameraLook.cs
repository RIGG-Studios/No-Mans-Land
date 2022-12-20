using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class CameraLook : MonoBehaviour
{
 public bool CanLook { get; set; }
    
    [SerializeField] private Transform camTransform;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 cameraBaseOffset;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float lookSmooth;
    [SerializeField] private float maxLookAngleY;

    
    private Vector2 _lookRotation;
    private Quaternion _nativeRotation;

    private float _lookX;
    private float _lookY;
    
    private float _fov;
    private float _defaultFOV;

    private Camera _camera;

    public Quaternion PlayerRotation { get; private set; }
    public Quaternion CameraRotation { get; private set; }

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _lookRotation.x = playerTransform.eulerAngles.y;
        _lookRotation.y = camTransform.eulerAngles.y;

        _nativeRotation.eulerAngles = new Vector3(0f, _lookRotation.y, 0f);

        _camera = GetComponentInChildren<Camera>();
        _defaultFOV = _camera.fieldOfView;
        _fov = _defaultFOV;
        CanLook = true;
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
        float nextHorizontal = _lookX * Time.deltaTime * mouseSensitivity;
        float nextVertical = _lookY * Time.deltaTime * mouseSensitivity;

        _lookRotation.x += nextHorizontal;
        _lookRotation.y += nextVertical;

        _lookRotation.y = Mathf.Clamp(_lookRotation.y, -maxLookAngleY, maxLookAngleY);
        
        Quaternion camTargetRotation = _nativeRotation * Quaternion.AngleAxis(_lookRotation.y + (0), Vector3.left);
        Quaternion bodyTargetRotation = _nativeRotation * Quaternion.AngleAxis(_lookRotation.x + (0), Vector3.up);
        CameraRotation =  Quaternion.Slerp(camTransform.localRotation, camTargetRotation, lookSmooth);

        camTransform.localRotation = CameraRotation;
        PlayerRotation = Quaternion.Slerp(playerTransform.localRotation, bodyTargetRotation, lookSmooth);
        _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _fov, Time.deltaTime * 5f);
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
