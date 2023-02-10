using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using Random = UnityEngine.Random;

public class LocalFloater : MonoBehaviour
{
    [SerializeField] private float depthBeforeSubmerged;
    [SerializeField] private float displacementAmount;
    [SerializeField] private Rigidbody rigidBody;

    private float WaveHeight { get; set; }

    private WaterSearchParameters _searchParameters;
    private WaterSearchResult _searchResult;

    private WaterSurface _waterSurface;


    private void Awake()
    {
        _waterSurface = FindFirstObjectByType<WaterSurface>();
    }


    public void FixedUpdate()
    {
        _searchParameters.startPositionWS = _searchResult.candidateLocationWS;
        _searchParameters.targetPositionWS = transform.position;
        _searchParameters.error = 0.01f;
        _searchParameters.maxIterations = 8;

        if (_waterSurface.ProjectPointOnWaterSurface(_searchParameters, out _searchResult))
        {
            WaveHeight = _searchResult.projectedPositionWS.y;
        }
        
        if (!(transform.position.y < WaveHeight))
        {
            return;
        }
        
        float displacementMultiplier = Mathf.Clamp01((WaveHeight - transform.position.y) / depthBeforeSubmerged) *
                                       displacementAmount;

        rigidBody.AddForceAtPosition(new Vector3(0, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0),
            transform.position, ForceMode.Acceleration);
    }
}