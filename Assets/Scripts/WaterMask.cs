using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class WaterMask : MonoBehaviour
{
    [SerializeField] private Material waterMat;
    [SerializeField] private float radius;


    private void Update()
    {
        waterMat.SetVector("_AlphaMaskPos", new Vector4(transform.position.x, transform.position.y, transform.position.z, radius));

    }
}
