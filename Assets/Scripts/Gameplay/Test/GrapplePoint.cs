using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    public enum GrappleTypes
    {
        Pull,
        Swing
    }

    public GrappleTypes Type;
    
    public string LookAtID => "";
    public string ID => "GrapplePoint";

    public GameObject Point { get; private set; }

    private Rigidbody _shipRigidbody;

    private void Awake()
    {
        _shipRigidbody = transform.root.GetComponent<Rigidbody>();
    }

    public void LookAtInteract()
    {
    }

    public void StopLookAtInteract()
    {

    }

    public void OnGrappleInteract(Vector3 playerPos)
    {
        if (Type == GrappleTypes.Pull)
        {
            Vector3 dir = (_shipRigidbody.position-playerPos).normalized;

            Debug.Log(dir * 10f);
            _shipRigidbody.AddForceAtPosition(Point.transform.position,dir * 150f, ForceMode.Force);
        }
    }



    public bool ButtonInteract()
    {
        return false;
    }

    public void StopButtonInteract() { }
}
