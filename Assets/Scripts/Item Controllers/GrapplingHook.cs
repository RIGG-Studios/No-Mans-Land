using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : BaseWeapon
{
    [SerializeField] private Transform grapplePoint;
    [SerializeField] private float grappleDist;
    [SerializeField] private float jumpImpulse;
    
    private LineRenderer _lineRenderer;
    private SpringJoint _springJoint;
    private GrapplePoint _currentGrapplePoint;

    private Vector3 _grapplePoint;
    private Vector3 _currentGrapple;

    private Transform _playerCamera;
    private Transform _player;
    
    protected override void Awake()
    {
        base.Awake();
        
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        _playerCamera = Player.Camera.transform;
        _player = Player.transform;
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void StartGrapple()
    {
        RaycastHit hit;

        if (Physics.Raycast(_playerCamera.position, _playerCamera.forward, out hit, grappleDist))
        {
            _grapplePoint = hit.point;
            _springJoint = _player.gameObject.AddComponent<SpringJoint>();

            _springJoint.autoConfigureConnectedAnchor = false;
            _springJoint.connectedAnchor = _grapplePoint;


            float dist = (_player.position - _grapplePoint).magnitude;
            _springJoint.maxDistance = dist * .7f;
            _springJoint.minDistance = dist * 0.3f;

            _springJoint.spring = 6.0f;
            _springJoint.damper = 4.0f;
            _springJoint.massScale = 6.5f;
            _lineRenderer.positionCount = 2;
            
            Vector3 dir = (_grapplePoint - _playerCamera.transform.position).normalized;
          //  Player.Movement.AddForce(dir * jumpImpulse, ForceMode.Acceleration);
        }

    }

    private void DrawRope()
    {
        if (!_springJoint)
        {
            return;
        }
        
        _currentGrapple = Vector3.Lerp(_currentGrapple, _grapplePoint, Time.deltaTime * 15f);
        _lineRenderer.SetPosition(0, grapplePoint.position);
        _lineRenderer.SetPosition(1, _currentGrapple);
    }

    private void StopGrapple()
    {
        _lineRenderer.positionCount = 0;
        _currentGrapple = grapplePoint.forward;
        Destroy(_springJoint);
    }
}
