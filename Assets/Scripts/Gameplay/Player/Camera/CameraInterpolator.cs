using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class CameraInterpolator : MonoBehaviour
{
    [SerializeField] private float speed;
    
    private InterpolationManager _interpolationManager;
    
    
    private bool _isCompleted;

    private InterpolationData _interpolation;

    private void Awake()
    {
        _interpolationManager = GetComponent<InterpolationManager>();
    }

    
    public void InterpolateToTarget(InterpolationData interpolation)
    {
        if (!interpolation.IsValid)
        {
            return;
        }

        _interpolation = interpolation;

        StartCoroutine(Interpolate(interpolation));
    }

    private IEnumerator Interpolate(InterpolationData interpolation)
    {
        float t = 0.0f;

        while (t <= 1.0f)
        {
            t += Time.deltaTime * speed;
            
            transform.position = Vector3.Lerp(transform.position, interpolation.TargetPos, t);
            transform.rotation = Quaternion.Lerp(transform.rotation, interpolation.TargetRot, t);
            yield return null;
        }
        
    }

    public void InterpolateToDefault()
    {
        _interpolation = default;
        transform.localPosition = Vector3.zero;
        transform.localRotation = quaternion.identity;
    }
}
