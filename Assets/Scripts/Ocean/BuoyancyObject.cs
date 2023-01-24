using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class BuoyancyObject : NetworkBehaviour
{
    public Transform OvverideCenterOfMass;
    public Transform[] points;
    public float intensity;
    public float angularDrag = 0.25f;
    public float drag = 0.25f;
    
    public Rigidbody rigidBody { get; private set; }
    
    
    
    private void OnEnable()
    {
        rigidBody = GetComponent<Rigidbody>();


        Quaternion originalRotation = transform.rotation;
        Vector3 originalPosition = transform.position;
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
        if (OvverideCenterOfMass)
        {
            rigidBody.centerOfMass = transform.InverseTransformPoint(OvverideCenterOfMass.transform.position);
        }
        //else rigidBody.centerOfMass = new Vector3(0, -bounds.extents.y * 0f, 0) + transform.InverseTransformPoint(bounds.center);

        rigidBody.angularDrag = angularDrag;
        rigidBody.drag = drag;
        
        transform.rotation = originalRotation;
        transform.position = originalPosition;
    }
    
    
    public override void FixedUpdateNetwork()
    {
        /*/
        for (int i = 0; i < points.Length; i++)
        {
            float waterHeight = Ocean.Instance.GetWaterHeightAtPosition(points[i].position);
            
            Vector3 force = Vector3.zero;

            float k = points[i].position.y - waterHeight;
            
            if (k > 1)
            {
                k = 1f;
            }
            else if (k < 0)
            {
                k = 0;
                force =  -Physics.gravity * intensity;
                rigidBody.AddForceAtPosition(force, points[i].transform.position, ForceMode.Acceleration);
            }
        }
        /*/
    }
}

