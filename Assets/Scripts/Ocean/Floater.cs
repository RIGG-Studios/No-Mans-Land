using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using Random = UnityEngine.Random;

public class Floater : NetworkBehaviour
{
    [SerializeField] private float depthBeforeSubmerged;
    [SerializeField] private float displacementAmount;
    [SerializeField] private float errorCorrection;
    [SerializeField] private int maxIterations;
    [SerializeField] private Rigidbody rigidBody;

    [Networked]
   private float WaveHeight { get; set; }

   private WaterSearchParameters _waterSearch;
   private WaterSearchResult _waterResult;
   
   private WaterSurface _waterSurface;
   
   private void Awake()
   {
       _waterSurface = FindObjectOfType<WaterSurface>();
   }

   public override void FixedUpdateNetwork()
   {
       _waterSearch.startPositionWS = _waterResult.candidateLocationWS;
       _waterSearch.targetPositionWS = transform.position;
       _waterSearch.error = 0.01f;
       _waterSearch.maxIterations = 8;
       
       if (_waterSurface.ProjectPointOnWaterSurface(_waterSearch, out WaterSearchResult result))
       {
           WaveHeight = result.projectedPositionWS.y;
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
