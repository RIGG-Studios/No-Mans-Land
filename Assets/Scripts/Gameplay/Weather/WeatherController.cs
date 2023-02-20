using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class WeatherController : MonoBehaviour
{
    [SerializeField, Range(0.021f, 1)] 
    private float blendAmount;

    [Space]
    
    [SerializeField] private Light sunLight;
    [SerializeField] private Volume cloudVolume;
    [SerializeField] private Volume fogVolume;
    [SerializeField] private WaterSurface ocean;

    [Space] 
    
    [SerializeField] private float sparseLightIntensity;
    [SerializeField] private float stormLightIntensity;

    [Space]
    
    [SerializeField] private float sparseFogIntensity;
    [SerializeField] private float stormFogIntensity;

    [Space]
    
    [SerializeField] private float sparseOceanIntensity;
    [SerializeField] private float stormOceanIntensity;

    [SerializeField] private Transform rainTransform;
    [SerializeField] private Vector3 offset;

    private float _lightSourceIntensity;
    private float _cloudBlend;
    private float _fogIntensity;
    private float _oceanIntensity;

    private Fog _fog;
    private bool _setup;

    private void Setup()
    {
        fogVolume.profile.TryGet(out _fog);
        _setup = true;
    }

    #if UNITY_EDITOR
    public void OnValidate()
    {
        if (!_setup)
        {
            Setup();
        }
        
        Simulate();
    }
    
    #endif

    private void Update()
    { 
        Simulate();
    }

    private void Simulate()
    {
        _cloudBlend = blendAmount;
        _lightSourceIntensity = Mathf.Lerp(sparseLightIntensity, stormLightIntensity, blendAmount);
        _fogIntensity = Mathf.Lerp(sparseFogIntensity, stormFogIntensity, blendAmount);
        _oceanIntensity = Mathf.Lerp(sparseOceanIntensity, stormOceanIntensity, blendAmount);
        
        UpdateWeather();

        if (blendAmount >= 0.95f)
        {
            rainTransform.gameObject.SetActive(true);
            UpdateRain();
        }
        else
        {
            rainTransform.gameObject.SetActive(false);
        }
    }

    private void UpdateWeather()
    {
        sunLight.intensity = _lightSourceIntensity;
        cloudVolume.weight = _cloudBlend;
        _fog.meanFreePath.value = _fogIntensity;
        ocean.largeWindSpeed = _oceanIntensity;
    }

    private void UpdateRain()
    {
        if (NetworkPlayer.Local == null)
        {
            return;
        }

        rainTransform.position = NetworkPlayer.Local.transform.position + offset;
    }
}
