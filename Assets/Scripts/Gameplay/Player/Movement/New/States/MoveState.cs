using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class MoveState : State
{
    protected float Gravity;
    protected float MovementSpeed;
    protected Vector3 SlopeNormal;
    protected Rigidbody Rigidbody;
    protected Transform Transform;
    protected NetworkRunner Runner;
    protected CameraLook CameraController;

    public override void Init(PlayerMovementHandler movementHandler, StateTypes type)
    {
        base.Init(movementHandler, type);
        
        Gravity = movementHandler.gravity;
        MovementSpeed = movementHandler.movementSpeed;

        Rigidbody = movementHandler.GetComponent<Rigidbody>();
        Transform = movementHandler.transform;
        Runner = movementHandler.Runner;
        CameraController = movementHandler.cameraLook;
    }

    public override void OnUpdate()
    {
        CameraController.SetFOV(0, true);
    }

    public override void Move(NetworkInputData input)
    {
        UpdatePosition(input);
        UpdateRotation(input);
    }
    
    private void UpdateRotation(NetworkInputData networkInputData)
    {
        Transform.rotation = Quaternion.Slerp(Transform.rotation, networkInputData.LookForward, 1f);
    }
    
    private void UpdatePosition(NetworkInputData networkInputData)
    {
        Vector2 movementInput = networkInputData.MovementInput.normalized;

        Rigidbody.AddForce(Physics.gravity * Gravity, ForceMode.Force);
        Rigidbody.useGravity = true;

        Vector3 forward = Transform.forward;
            
        if (OnSlope())
        {
            forward = Vector3.ProjectOnPlane(forward, SlopeNormal);
        }

        Vector3 nextPos = Transform.position +
                          (forward * movementInput.y + Transform.right * movementInput.x) * MovementSpeed *
                          Runner.DeltaTime;
            
        Rigidbody.MovePosition(nextPos);
    }
    
    private bool OnSlope()
    {
        RaycastHit hit;

        if (Physics.Raycast(Transform.position, Vector3.down, out hit, 2 * 0.5f + 0.5f))
        {
            if (hit.normal != Vector3.up)
            {
                SlopeNormal = hit.normal;
                return true;
            }
        }

        return false;
    }
}
