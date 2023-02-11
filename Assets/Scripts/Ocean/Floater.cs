using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class Floater : NetworkBehaviour
{
    [SerializeField] private float depthBeforeSubmerged;
    [SerializeField] private float displacementAmount;
    [SerializeField] private Rigidbody rigidBody;
    
    
    [Networked]
   private float WaveHeight { get; set; }

   private void Awake()
    {
        rigidBody.useGravity = false;
    }
    
    public override void FixedUpdateNetwork()
    {
        rigidBody.useGravity = true;

        if (!(transform.position.y < WaveHeight))
        {
            return;
        }

        WaveHeight = Ocean.Instance.GetWaterHeightAtPosition(transform.position);

        float displacementMultiplier = Mathf.Clamp01((WaveHeight - transform.position.y) / depthBeforeSubmerged) *
                                       displacementAmount;

        rigidBody.AddForceAtPosition(new Vector3(0, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0),
            transform.position, ForceMode.Acceleration);
    }
}
