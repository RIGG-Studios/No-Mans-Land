using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class Tiling : MonoBehaviour
{
    private enum ScalingFactor
    {
        InverseGoldenRatio,
        GoldenRatio
    }
    
    [Header("Ocean")]
    [SerializeField] private Ocean oceanManager;

    [Header("Shaders")]
    [SerializeField] private Material highDetailShader;
    [SerializeField] private Material mediumDetailShader;
    [SerializeField] private Material lowDetailShader;

    [Header("Cascades")]
    [SerializeField] private float largestLength;
    [SerializeField] private ScalingFactor scalingFactor;

    private Cascade[] _cascades;

    private float _time;
    private readonly int _heightMapProp = Shader.PropertyToID("_HeightMap");
    private readonly int _normalMapProp = Shader.PropertyToID("_NormalMap");
    private readonly int _foldingMapProp = Shader.PropertyToID("_FoldingMap");

    private void Awake()
    {
        _cascades = new Cascade[3];
        highDetailShader.SetFloat("_Resolution", oceanManager.resolution);
        mediumDetailShader.SetFloat("_Resolution", oceanManager.resolution);
        lowDetailShader.SetFloat("_Resolution", oceanManager.resolution);
    }

    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            _cascades[i].lengthScale = ComputeLengthScale(i);
            _cascades[i].largestWaveNumber = i == 2 ? 99999.99f :
                math.sqrt(math.PI * 2) * oceanManager.resolution / _cascades[i].lengthScale;
            
            _cascades[i].InitializeTextures(oceanManager.resolution);

            oceanManager.ComputeInitialSpectrum(ref _cascades[i]);
        }
    }

    void Update()
    {
        _time += Time.deltaTime;
        
        oceanManager.ComputeFourierComponents(ref _cascades, _time);
        oceanManager.ComputeHeightNormalFoldingMap(ref _cascades, largestLength);
        
        highDetailShader.SetTexture(_heightMapProp, _cascades[0].heightMap);
        highDetailShader.SetTexture(_normalMapProp, _cascades[0].normalMap);
        highDetailShader.SetTexture(_foldingMapProp, _cascades[0].foldingMap);
    }
    
    //Generates a length scale according to the formula Li = s^i * L
    private float ComputeLengthScale(int cascadeNumber)
    {
        float goldenRatio = (1 + math.sqrt(5))/2.0f;

        float scaling = scalingFactor switch
        {
            ScalingFactor.GoldenRatio => math.pow(goldenRatio, -1),
            ScalingFactor.InverseGoldenRatio => 1 - math.pow(goldenRatio, -1),
            _ => 0
        };

        return math.pow(scaling, cascadeNumber) * largestLength;
    }
}

[System.Serializable]
public struct Cascade
{
    //Smallest is inclusive, largest is exclusive
    public float largestWaveNumber;
    public float lengthScale;
    
    public RenderTexture h0;
    public RenderTexture hk;
    public RenderTexture hk2;
    public RenderTexture nk;

    public RenderTexture heightMap;
    public RenderTexture normalMap;
    public RenderTexture foldingMap;
    public RenderTexture pingPong;

    public void InitializeTextures(int resolution)
    {
        h0 = new RenderTexture(resolution, resolution, 0, GraphicsFormat.R16G16B16A16_SFloat);
        hk = new RenderTexture(resolution, resolution, 0, GraphicsFormat.R16G16B16A16_SFloat);
        hk2 = new RenderTexture(resolution, resolution, 0, GraphicsFormat.R16G16B16A16_SFloat);
        nk = new RenderTexture(resolution, resolution, 0, GraphicsFormat.R16G16B16A16_SFloat);
        pingPong = new RenderTexture(resolution, resolution, 0, GraphicsFormat.R16G16B16A16_SFloat);
        
        heightMap = new RenderTexture(resolution, resolution, 0, GraphicsFormat.R16G16B16A16_SFloat);
        normalMap = new RenderTexture(resolution, resolution, 0, GraphicsFormat.R16G16B16A16_SFloat);
        foldingMap = new RenderTexture(resolution, resolution, 0, GraphicsFormat.R16G16B16A16_SFloat);

        h0.enableRandomWrite = true;
        hk.enableRandomWrite = true;
        hk2.enableRandomWrite = true;
        nk.enableRandomWrite = true;
        heightMap.enableRandomWrite = true;
        normalMap.enableRandomWrite = true;
        foldingMap.enableRandomWrite = true;
        pingPong.enableRandomWrite = true;
    }
}
