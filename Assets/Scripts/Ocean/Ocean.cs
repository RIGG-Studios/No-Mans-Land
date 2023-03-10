using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class Ocean : NetworkBehaviour
{
    public static Ocean Instance;

    [Header("MATERIAL SETTINGS")] 
    
    [SerializeField] private Material oceanMaterial;
    [SerializeField] private float waveTiling;

    [Header("SHADER SETTINGS")]
        
    [SerializeField] private bool executeInEditor = true;
    [SerializeField] private OceanPresets oceanPreset;
    
    
    private const string ShallowWaterColorID = "_Color01";
    private const string DeepWaterColorID = "_Color02";
    private const string FoamColorID = "_FoamColor";
    private const string ExtraDispersionID = "_ExtraExpersion";
    private const string FoamEdgeHardnessID = "_FoamEdgeHardness";
    private const string CrestSizeID = "_CrestSize";
    private const string CrestOffsetID = "_CrestOffset";
    private const string FoamFalloffID = "_FoamFalloff";
    private const string FoamWidthID = "_FoamWidth";
    private const string FoamRemovalID = "_FoamRemoval";
    private const string FoamBandsID = "_FoamBands";
    
    private OceanPresets _oceanPreset;

    
    [Networked]
    public float Time { get; set; }
    
    private void Awake()
    {
        Instance = this;
    }

    public Material GetOceanMat()
    {
        return oceanMaterial;
    }


    private void OnValidate()
    {
        if (!executeInEditor)
            return;

        UpdateShaderProperties();
    }

    public void UpdateShaderProperties()
    {
        if (oceanMaterial == null)
            return;


        //waves
        foreach (WaveSettings wave in oceanPreset.waves.waves)
        {
            oceanMaterial.SetFloat(wave.amplitudeID, wave.amplitude);
            oceanMaterial.SetFloat(wave.steepnessID, wave.steepness);
            oceanMaterial.SetFloat(wave.frequencyID, wave.frequency);
            oceanMaterial.SetFloat(wave.speedID, wave.speed);
            oceanMaterial.SetVector(wave.directionID, wave.direction);
        }
        
        //colors
        oceanMaterial.SetColor(ShallowWaterColorID,  oceanPreset.shallowColor);
        oceanMaterial.SetColor(DeepWaterColorID,  oceanPreset.deepColor);
        oceanMaterial.SetColor(FoamColorID,  oceanPreset.foamColor);
        
        //wave foam
        oceanMaterial.SetFloat(ExtraDispersionID, oceanPreset.extraDispersion);
        oceanMaterial.SetFloat(FoamEdgeHardnessID, oceanPreset.edgeHardness);
        oceanMaterial.SetFloat(CrestSizeID, oceanPreset.crestSize);
        oceanMaterial.SetFloat(CrestOffsetID, oceanPreset.crestOffset);
        
        //edge foam
        oceanMaterial.SetFloat(FoamFalloffID, oceanPreset.foamFallOff);
        oceanMaterial.SetFloat(FoamWidthID, oceanPreset.foamWidth);
        oceanMaterial.SetFloat(FoamRemovalID, oceanPreset.foamRemoval);
        oceanMaterial.SetFloat(FoamBandsID, oceanPreset.foamBands);
    }

    public override void FixedUpdateNetwork()
    {
        Time += Runner.DeltaTime;
        oceanMaterial.SetFloat("_GameTime", Time);
    }


    public float GetWaterHeightAtPosition(Vector3 pos)
    {
        float y = 0.0f;
        foreach (WaveSettings wave in oceanPreset.waves.waves)
        {
            y += CalculateWaveHeight(pos, wave);
        }

        return y * waveTiling;
    }

    private float CalculateWaveHeight(Vector3 pos, WaveSettings waveSettings)
    {
        Vector2 dir = waveSettings.direction.normalized;
        Vector2 negatedDir = -1 * dir;

        Vector2 frequencyDir = negatedDir * waveSettings.frequency;

        float dot = Vector2.Dot(new Vector2(pos.x, pos.z), frequencyDir);
        float time = Time * waveSettings.speed;
        float total = dot + time;
            float amp = waveSettings.amplitude * waveSettings.steepness;

        return Mathf.Cos(total) * (amp * dir.y);
    }
    
}