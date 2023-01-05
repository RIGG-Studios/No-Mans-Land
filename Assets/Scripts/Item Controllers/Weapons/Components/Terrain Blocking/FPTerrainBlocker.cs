using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FPTerrainBlocker : MonoBehaviour
{
    public bool IsBlocked { get; private set; }
    
    [SerializeField] private Transform blockTransform;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float itemLength;
    [SerializeField] private Vector3 blockedPos;
    [SerializeField] private Vector3 blockedRot;


    private Vector3 _currentPos;
    private Quaternion _currentRot;

    private Transform _camTransform;


    private void Start()
    {
        _camTransform = NetworkPlayer.Local.Camera.transform;
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(_camTransform.position, _camTransform.forward, out hit, itemLength))
        {
            _currentPos = blockedPos;
            _currentRot = Quaternion.Euler(blockedRot);
            IsBlocked = true;
        }
        else
        {
            _currentPos = Vector3.zero;
            _currentRot = Quaternion.identity;
            IsBlocked = false;
        }


        blockTransform.localPosition =
            Vector3.Lerp(blockTransform.localPosition, _currentPos, Time.deltaTime * movementSpeed);

        blockTransform.localRotation =
            Quaternion.Lerp(blockTransform.localRotation, _currentRot, Time.deltaTime * 5f);
    }
}
