using Fusion;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public bool CanLook { get; set; }
    
    [SerializeField] private Transform camTransform;
    [SerializeField] private NetworkPlayer player;
    [SerializeField] private Vector3 cameraBaseOffset;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float lookSmooth;
    [SerializeField] private float maxLookAngleY; 
    
    
    
    private float _maxLookAngleX;
    private float _minLookAngleX;

    
    private Vector2 _lookRotation;
    private Quaternion _nativeRotation;

    private float _lookX;
    private float _lookY;
    
    private float _fov;
    private float _defaultFOV;
    private bool _lockHorizontalMovement;

    public Camera Camera { get; private set; }
    public CameraInterpolator Interpolator { get; private set; }
    public Quaternion PlayerRotation { get; private set; }
    public Quaternion CameraRotation { get; private set; }
    
    public float RawLookX { get; private set; }
    public float RawLookY { get; private set; }

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = true;
        _lookRotation.x = player.transform.eulerAngles.y;
        _lookRotation.y = camTransform.eulerAngles.y;

        _nativeRotation.eulerAngles = new Vector3(0f, _lookRotation.y, 0f);

        Camera = GetComponentInChildren<Camera>();
        Interpolator = GetComponentInChildren<CameraInterpolator>();
        _defaultFOV = Camera.fieldOfView;
        _fov = _defaultFOV;
        CanLook = true;
    }

    public void ToggleCamera(bool state) => Camera.gameObject.SetActive(state);
    
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

        RawLookX = nextHorizontal;
        RawLookY = nextVertical;
        
        
        _lookRotation.x += nextHorizontal;
        _lookRotation.y += nextVertical;
        
        _lookRotation.y = Mathf.Clamp(_lookRotation.y, -maxLookAngleY, maxLookAngleY);

        if (_lockHorizontalMovement)
        {
            _lookRotation.x = Mathf.Clamp(_lookRotation.x, _minLookAngleX, _maxLookAngleX);
        }

        Quaternion camTargetRotation = _nativeRotation * Quaternion.AngleAxis(_lookRotation.y + (0), Vector3.left);
        Quaternion bodyTargetRotation = _nativeRotation * Quaternion.AngleAxis(_lookRotation.x + (0), Vector3.up);
        CameraRotation =  Quaternion.Slerp(camTransform.localRotation, camTargetRotation, lookSmooth);

        camTransform.localRotation = CameraRotation;
        PlayerRotation = Quaternion.Slerp(player.transform.localRotation, bodyTargetRotation, lookSmooth);
        Camera.fieldOfView = Mathf.Lerp(Camera.fieldOfView, _fov, Time.deltaTime * 5f);
    }
    
    
    public void UpdateLookDirection(float hor, float ver)
    {
        _lookY = ver;
        _lookX = hor;
    }

    public void UpdateHorizontalLock(float minLook, float maxLook, bool lockRot)
    {
        _lockHorizontalMovement = lockRot;
        _minLookAngleX = minLook;
        _maxLookAngleX = maxLook;
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

    public void UpdateRecoil(float x, float y)
    {
        _lookRotation.x += x;
        _lookRotation.y += y;
    }

    public void Fall()
    {
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
    }
}
