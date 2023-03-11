using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Rendering.HighDefinition;
using Object = UnityEngine.Object;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 7;
    
    [Header("Jumping")]
    [SerializeField] private float jumpSpeed = 10f;
    
    [Header("Sprinting")]
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float sprintFOV = 110f;
    
    [Header("Climbing")]
    [SerializeField] private float climbSpeed = 5f;

    [Header("Swimming")]
    [SerializeField] private float swimSpeed;
    [SerializeField] private float swimDrag;
    [SerializeField] private float swimUpSpeed;
    [SerializeField] private float bouyancy;
    
    [Header("Other")]
    [SerializeField] private float gravity = 10f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float groundCheckDist = 0.4f;

    private Rigidbody _rigidbody;
    private Vector3 _slopeNormal;

    private float _currentSpeed;
    private float _defaultDrag;

    private Vector3 _lastWaveForceForward;
    private Vector3 _lastWaveForceRight;

    public float waveVelocityCorrection;
    public float forceHeightOffset;
    public float dragInWaterRight;
    public float dragInWaterForward;
    public float waterSurfaceFloatThreshold;
    public float test;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _currentSpeed = movementSpeed;
        _defaultDrag = _rigidbody.drag;
    }

    public void Move(NetworkInputData input, float runnerTime)
    {
        _rigidbody.useGravity = true;
        _rigidbody.drag = _defaultDrag;
        Vector2 movementInput = input.MovementInput.normalized;

        _rigidbody.AddForce(Physics.gravity * gravity, ForceMode.Force);

        Vector3 forward = transform.forward;
            
        if (OnSlope())
        {
            forward = Vector3.ProjectOnPlane(forward, _slopeNormal);
        }

        Vector3 nextPos = transform.position +
                          (forward * movementInput.y + transform.right * movementInput.x) * _currentSpeed *
                          runnerTime;
            
        _rigidbody.MovePosition(nextPos);
    }

    public void MoveLadder(NetworkInputData input, float runnerTime)
    {
        _rigidbody.useGravity = false;
        
        Vector2 movementInput = input.MovementInput.normalized;

        Vector3 forward = transform.up;

        Vector3 nextPos = transform.position +
                          (forward * movementInput.y + transform.right * movementInput.x) * _currentSpeed *
                          runnerTime;
        
        _rigidbody.MovePosition(nextPos);
    }
    
    public void MoveSwim(NetworkInputData input, bool isRunning, float runnerTime, WaterSearchResult currentFrameResult)
    {
        _rigidbody.useGravity = false;
        _rigidbody.drag = Mathf.Lerp(_rigidbody.drag, swimDrag, runnerTime * 2.0f);

        Vector3 forward = (input.LookForward * input.LookVertical) * Vector3.forward;
        
        float speed = swimSpeed * (isRunning ? 2.0f : 1f);

        Vector3 nextPos = transform.position +
                          (forward * input.MovementInput.y + transform.right * input.MovementInput.x) * speed *
                          runnerTime;


        CalculateWaterResponse(currentFrameResult);
        bool isMoving = input.MovementInput.y != 0.0f || input.MovementInput.x != 0.0f;
        
        if (!isMoving && currentFrameResult.projectedPositionWS.y - transform.localPosition.y < waterSurfaceFloatThreshold)
        {
            nextPos.y += (currentFrameResult.projectedPositionWS.y - transform.localPosition.y) * test;
        }

        if (input.IsSpacePressed)
        {
            _rigidbody.AddForce(Vector3.up * 10.0f, ForceMode.Force);
        }
        
        _rigidbody.MovePosition(nextPos);

    }

    private void CalculateWaterResponse(WaterSearchResult currentFrameResult)
    {
        Vector3 pos = currentFrameResult.projectedPositionWS;
        Vector3 waveDir = pos - transform.position;
        Debug.Log(waveDir);
        
        
        
        Vector3 displacement = (pos - transform.position);
        Vector3 dir = transform.position - displacement;
        dir.y = 0;

        Vector3 velocityRelativeToWater = _rigidbody.velocity - waveDir * waveVelocityCorrection;
        Vector3 forcePos = _rigidbody.position + forceHeightOffset * Vector3.up;

        _lastWaveForceRight =
            transform.right * Vector3.Dot(transform.right, -velocityRelativeToWater) * dragInWaterRight;

        _lastWaveForceForward = transform.forward * Vector3.Dot(transform.forward, -velocityRelativeToWater) *
                              dragInWaterForward;
        
        _rigidbody.AddForceAtPosition(_lastWaveForceRight, forcePos, ForceMode.Acceleration);
        _rigidbody.AddForceAtPosition(_lastWaveForceForward, forcePos, ForceMode.Acceleration);
    }

    public void UpdateCameraRotation(NetworkInputData input)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, input.LookForward, 1f);
    }

    public void Jump()
    {
        _rigidbody.AddForce(transform.up * jumpSpeed, ForceMode.Impulse);
    }

    public void ToggleSprint(bool isSprinting)
    {
        _currentSpeed = isSprinting ? sprintSpeed : movementSpeed;
    }
    
    private bool OnSlope()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2 * 0.5f + 0.5f))
        {
            if (hit.normal != Vector3.up)
            {
                _slopeNormal = hit.normal;
                return true;
            }
        }

        return false;
    }
}
