using System;
using Fusion;
using UnityEngine;


public class PlayerMovementHandler : NetworkBehaviour
{
    public float movementSpeed = 7;
    public float jumpSpeed = 10f;
    public float sprintSpeed = 10f;
    public float sprintFOV = 110f;

    
    public bool IsMovingForward { get; private set; }
    
    private bool _canSendInputToStates = true;
    private Rigidbody _rigidbody;
    private float _vertical;
    private float _horizontal;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        IsMovingForward = _vertical >= 0.5f;
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData networkInputData))
        {
            return;
        }
        
        Debug.Log(networkInputData.LookForward);
        transform.forward = networkInputData.LookForward;
        
        Quaternion rotation = transform.rotation;
        rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
        transform.rotation = rotation;
        
        Vector2 movementInput = networkInputData.MovementInput.normalized;

        _horizontal = movementInput.x;
        _vertical = movementInput.y;

        Vector3 nextPos = transform.position +
                          (transform.forward * _vertical + transform.right * _horizontal) * movementSpeed *
                          Runner.DeltaTime;
        
        _rigidbody.MovePosition(nextPos);
    }
}
