using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class DamageNotifier : MonoBehaviour
{
    public static DamageNotifier Instance;
    
    [SerializeField] private GameObject hitMarkerAnimator;

    private void Awake()
    {
        Instance = this;
    }

    public void OnDamageEntity(Vector3 pos, float damage)
    {
        GameObject g =  Instantiate(hitMarkerAnimator, pos, quaternion.identity);
        
        Destroy(g, 4.0f);
    }
}
