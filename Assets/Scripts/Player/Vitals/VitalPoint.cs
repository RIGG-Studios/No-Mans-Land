using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VitalPoint : MonoBehaviour
{
    [SerializeField] private bool findOnRoot = true;

    private NetworkHealthHandler _healthHandler;


    private void Awake()
    {
        if (findOnRoot)
        {
            _healthHandler = transform.root.GetComponent<NetworkHealthHandler>();
        }
        else
        {
            _healthHandler = GetComponent<NetworkHealthHandler>();
        }

        if (_healthHandler == null)
        {
            Debug.Log("Couldn't find a network health handler");
        }
    }
}
