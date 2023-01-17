using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    [Header("Other")]
    [SerializeField] private float gravity = 10f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float groundCheckDist = 0.4f;

    private Rigidbody _rigidbody;
    private Vector3 _slopeNormal;

    private float _currentSpeed;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _currentSpeed = movementSpeed;
    }

    public void Move(NetworkInputData input, float runnerTime)
    {
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
