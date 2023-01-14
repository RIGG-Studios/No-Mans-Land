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

    public void OnDamageEntity(Transform target, Vector3 pos, float damage)
    {
        Vector3 normal = Vector3.Cross(target.position, pos);

        GameObject g = Instantiate(hitMarkerAnimator, pos, Quaternion.LookRotation(normal));
        g.transform.LookAt(target);
        
        
        Destroy(g, 4.0f);
    }
}
