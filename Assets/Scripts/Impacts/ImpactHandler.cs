using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ImpactHandler : NetworkBehaviour
{
    public static ImpactHandler Instance;
    
    [SerializeField] private ImpactProperties[] impacts;

    [System.Serializable]
    public struct ImpactProperties
    {
        public string tag;
        public NetworkObject impactPrefab;
    }


    private void Awake()
    {
        Instance = this;
    }

    public void RequestImpact(string tag, Vector3 position, Vector3 normal)
    {
        if (!Runner.IsServer)
        {
            return;
        }
        
        ImpactProperties impactProperties = FindImpact(tag);

        if (impactProperties.impactPrefab == null)
        {
            return;
        }


        Impact impact = Runner.Spawn(impactProperties.impactPrefab, position, Quaternion.LookRotation(normal)).GetComponent<Impact>();
        impact.Init();
    }


    private ImpactProperties FindImpact(string tag)
    {
        for (int i = 0; i < impacts.Length; i++)
        {
            if (tag == impacts[i].tag)
            {
                return impacts[i];
            }
        }

        return default;
    }
}
