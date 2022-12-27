using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class DamageNotifier : MonoBehaviour
{
    [SerializeField] private GameObject hitMarkerAnimator;

    private readonly int _show = Animator.StringToHash("Show");

    private bool _lerp;
    private Vector3 _pos;
    
    
    private void Update()
    {

    }

    public void OnDamageEntity(Vector3 pos, float damage)
    {
        _pos = pos;
        _lerp = true;


        GameObject g =  Instantiate(hitMarkerAnimator, pos + -Camera.main.transform.forward, quaternion.identity);
        
        Destroy(g, 4.0f);
    }
}
