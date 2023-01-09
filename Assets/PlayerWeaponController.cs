using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerWeaponController : NetworkBehaviour
{
    private Rigidbody _rigidbody;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }


    public override void FixedUpdateNetwork()
    {
        _rigidbody.AddForce(transform.forward * 400f, ForceMode.Force);
    }
}
