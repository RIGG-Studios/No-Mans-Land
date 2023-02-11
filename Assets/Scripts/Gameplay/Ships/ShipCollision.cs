using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ShipCollision : SimulationBehaviour
{
    [SerializeField] private float velocity;
    [SerializeField] private ShipPhysicsHandler shipPhysics;
    
    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("hi");
        if (!Object.HasStateAuthority)
        {
            return;
        }

        Vector3 dir = (transform.position - collision.contacts[0].point).normalized;
        Debug.Log(dir);
        shipPhysics.AddForce(dir * velocity);
    }
}
