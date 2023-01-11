using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class Floater : NetworkBehaviour
{
    [SerializeField] private float seaLevel;
    [SerializeField] private float fakeAmplitude;
    [SerializeField] private float depthBeforeSubmerged;
    [SerializeField] private float displacementAmount;
    [SerializeField] private float waterDrag;
    [SerializeField] private float waterAngularDrag;
    [SerializeField] private Rigidbody rigidBody;

   // private float _randomPhase;
   // private float _waveHeight;
    
   [Networked]
   private float RandomPhase { get; set; }
   
   [Networked]
   private float WaveHeight { get; set; }
   
   [Networked]
   private NetworkBool SetUpPhase { get; set; }
    

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

        if (Object.HasStateAuthority && !SetUpPhase)
        {
            RandomPhase = Random.Range(-10.0f, 10.0f);
            SetUpPhase = true;
        }

        WaveHeight = fakeAmplitude * Mathf.Sin( WaveTime.Instance.Time + RandomPhase) + seaLevel;
        
        float displacementMultiplier = Mathf.Clamp01((WaveHeight - transform.position.y) / depthBeforeSubmerged) *
                                       displacementAmount;

        rigidBody.AddForceAtPosition(new Vector3(0, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0),
            transform.position, ForceMode.Acceleration);
        
        
        //comment out these lines to fix network bouyancy interpolation
        //add torque uses time.fixeddeltatime internally and that makes it uncompatiable with Fusion
        rigidBody.AddForce(-rigidBody.velocity * (displacementMultiplier * waterDrag * Runner.DeltaTime),
            ForceMode.VelocityChange);
        rigidBody.AddTorque(-rigidBody.angularVelocity * (displacementMultiplier * waterAngularDrag * Runner.DeltaTime),
            ForceMode.VelocityChange);
    }
}
