using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalWaveSync : MonoBehaviour
{
    [SerializeField] private Material oceanMat;


    private float _time = 0.0f;
    
    private void Update()
    {
        _time = Time.time;
        
        oceanMat.SetFloat("_GameTime", _time);
    }
}
