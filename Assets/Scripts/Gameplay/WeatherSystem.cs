using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class WeatherSystem : MonoBehaviour
{
    [SerializeField] private float foggyAmount;
    [SerializeField] private float defaultFog;
    [SerializeField] private float smoothing;
    
    private Volume _volume;

    private Fog _fog;

    private bool _foggy;
    private float _currentFog;

    private void Awake()
    {
        _volume = GetComponent<Volume>();
        _volume.profile.TryGet(out Fog fog);

        _fog = fog;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) _foggy = !_foggy;

        float targetFog = _foggy ? foggyAmount : defaultFog;
        _currentFog = Mathf.Lerp(_currentFog, targetFog, Time.deltaTime * smoothing);

        _fog.maximumHeight.value = _foggy ? 750f : 50f;
        _fog.meanFreePath.value = _currentFog;
    }
}
