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
    [SerializeField] private int floaterCount;
    [SerializeField] private float waterDrag;
    [SerializeField] private float waterAngularDrag;
    [SerializeField] private Rigidbody rigidBody;

    private float _randomPhase;

    private void Awake()
    {
        _randomPhase = Random.Range(-10.0f, 10.0f);
        rigidBody.useGravity = false;
    }
    

    public override void FixedUpdateNetwork()
    {
        rigidBody.useGravity = true;
        float waveHeight = fakeAmplitude * Mathf.Sin( Runner.DeltaTime + _randomPhase) + seaLevel;
        if (!(transform.position.y < waveHeight))  return;
        
        float displacementMultiplier = Mathf.Clamp01((waveHeight - transform.position.y) / depthBeforeSubmerged) * displacementAmount;
        
        rigidBody.AddForceAtPosition(new Vector3(0, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0), transform.position, ForceMode.Acceleration);
        rigidBody.AddForce(-rigidBody.velocity * (displacementMultiplier * waterDrag * Runner.DeltaTime), ForceMode.VelocityChange);
        rigidBody.AddTorque(-rigidBody.angularVelocity * (displacementMultiplier * waterAngularDrag *  Runner.DeltaTime), ForceMode.VelocityChange);
    }
}
