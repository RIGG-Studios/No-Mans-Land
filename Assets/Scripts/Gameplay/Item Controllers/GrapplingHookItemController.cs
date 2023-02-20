using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class GrapplingHookItemController : BaseWeapon
{

    [SerializeField] private Transform grapplePoint;
    [SerializeField] private float grappleDist;


    private LineRenderer _lineRenderer;
    private SpringJoint _springJoint;


    private Vector3 _grapplePoint;
    private Vector3 _currentGrapple;

    private bool _isGrappling;
    
    protected  override void Awake()
    {
        base.Awake();
        _lineRenderer = GetComponent<LineRenderer>();
    }


    public override void ProcessInput(WeaponContext context)
    {
        if (Object.IsProxy)
        {
            return;
        }
        
        if (context.Input.IsFiring)
        {
            Debug.Log("start g");
            StartGrapple(context.FirePosition, context.FireDirection);
        }
        else
        {
            Debug.Log("end g");
            EndGrapple();
        }
    }


    private void LateUpdate()
    {
        DrawRope();
    }


    private void StartGrapple(Vector3 firePos, Vector3 fireDir)
    {
        if (_isGrappling)
        {
            return;
        }
        
        if (!Runner.GetPhysicsScene().Raycast(firePos, fireDir, out RaycastHit hit,
                grappleDist))
        {
            return;
        }
        
        _grapplePoint = hit.point;
        _springJoint = Player.gameObject.AddComponent<SpringJoint>();

        _springJoint.autoConfigureConnectedAnchor = false;
        _springJoint.connectedAnchor = _grapplePoint;
        
        float dist = (Player.transform.position - _grapplePoint).magnitude;
        _springJoint.maxDistance = dist * .7f;
        _springJoint.minDistance = dist * 0.3f;

        _springJoint.spring = 9.0f;
        _springJoint.damper = 2.0f;
        _springJoint.massScale = 8.5f;
        _lineRenderer.positionCount = 2;
        _isGrappling = true;

        //   Vector3 dir = (_grapplePoint - _playerCamera.transform.position).normalized;
        //  Player.Movement.AddForce(dir * jumpImpulse, ForceMode.Acceleration);
    }

    private void EndGrapple()
    {
        if (Object.HasInputAuthority)
        {
            _lineRenderer.positionCount = 0;
        }

        _currentGrapple = grapplePoint.forward;
        Destroy(_springJoint);
        _isGrappling = false;
    }

    private void DrawRope()
    {
        if (!_springJoint)
        {
            return;
        }

        if (Player.Object.HasInputAuthority)
        {
            _currentGrapple = Vector3.Lerp(_currentGrapple, _grapplePoint, Time.deltaTime * 15f);
            _lineRenderer.SetPosition(0, grapplePoint.position);
            _lineRenderer.SetPosition(1, _currentGrapple);
        }
    }
}
