using System;
using UnityEngine;

public class ScaleByDistance : MonoBehaviour
{
    [SerializeField] private float scaleDistance;
    
    private Vector3 _startPos;


    public void Init(Vector3 startPos) => _startPos = startPos;


    private void Update()
    {
        float dist = (_startPos - transform.position).magnitude;

        transform.localScale = Vector3.one * (dist * scaleDistance);
    }
}
