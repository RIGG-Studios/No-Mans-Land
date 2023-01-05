using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class Ocean : MonoBehaviour
{
    [Header("Shaders")]
    public ComputeShader initialSpectrum;
    public ComputeShader fourierComponents;
    public ComputeShader butterflyPrecompute; 
    public ComputeShader butterflyOperations;
    public ComputeShader inversionAndPermutation;
    public ComputeShader foldingMapCompute;
    
    [Header("Ocean Parameters")]
    public Vector2 windDirection;
    public float windSpeed;
    public float windInfluence;
    public float amplitude;
    [Range(0, 1)]
    public float foamAmount;
    public float choppiness;
    public int resolution;
    public float smallestLength;

    //Gaussian random numbers
    [HideInInspector]
    public Texture2D zeta;

    //Textures used to perform the 2D iFFT
    private RenderTexture _butterfly;

    private void Awake()
    {
        zeta = new Texture2D(resolution, resolution, GraphicsFormat.R16G16B16A16_SFloat, TextureCreationFlags.None);
        
        //Compute gaussian random numbers
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                Vector2 randomGaussian = NextGaussian();
                zeta.SetPixel(i, j, new Color(randomGaussian.x, randomGaussian.y, 0));
            }
        }

        zeta.Apply();

        _butterfly = new RenderTexture((int)Mathf.Log(resolution, 2), resolution, 0, GraphicsFormat.R16G16B16A16_SFloat);
        _butterfly.enableRandomWrite = true;

        //Precompute butterfly texture with twiddle factors and indices
        butterflyPrecompute.SetInt("oceanResolution", resolution);
        butterflyPrecompute.SetTexture(0, "butterfly", _butterfly);
        butterflyPrecompute.Dispatch(0, (int)Mathf.Log(resolution, 2), resolution, 1);
    }

    public void ComputeInitialSpectrum(ref Cascade cascade)
    {
        initialSpectrum.SetTexture(0, "h0", cascade.h0);
        initialSpectrum.SetFloat("lengthScale", cascade.lengthScale);
        initialSpectrum.SetFloat("smallestLength", smallestLength);
        initialSpectrum.SetFloat("largestK", cascade.largestWaveNumber);
        initialSpectrum.SetInt("oceanResolution", resolution);
        
        initialSpectrum.SetFloat("windSpeed", windSpeed);
        initialSpectrum.SetFloats("windDirection", windDirection.normalized.x, windDirection.normalized.y);
        initialSpectrum.SetFloat("windInfluence", windInfluence);
        initialSpectrum.SetFloat("amplitude", amplitude * 1000);
        initialSpectrum.SetTexture(0, "zeta", zeta);
        
        initialSpectrum.Dispatch(0, resolution, resolution, 1);
    }

    public void ComputeFourierComponents(ref Cascade[] cascades, float time)
    {
        fourierComponents.SetInt("oceanResolution", resolution);
        fourierComponents.SetFloat("choppiness", choppiness);
        fourierComponents.SetFloat("time", time);
        
        for (int i = 0; i < cascades.Length; i++)
        {
            fourierComponents.SetTexture(0, "h0", cascades[i].h0);
            fourierComponents.SetTexture(0, "hk", cascades[i].hk);
            fourierComponents.SetTexture(0, "hk2", cascades[i].hk2);
            fourierComponents.SetTexture(0, "nk", cascades[i].nk);
            fourierComponents.SetFloat("lengthScale", cascades[i].lengthScale);
            
            fourierComponents.Dispatch(0, resolution, resolution, 1);
        }
    }
    
    public void ComputeHeightNormalFoldingMap(ref Cascade[] cascades, float lengthScale)
    {
        inversionAndPermutation.SetInt("oceanResolution", resolution);
        
        butterflyOperations.SetTexture(0, "pingPongOut", cascades[0].pingPong);
        butterflyOperations.SetTexture(0, "pingPongOut1", cascades[1].pingPong);
        butterflyOperations.SetTexture(0, "pingPongOut2", cascades[2].pingPong);
        
        butterflyOperations.SetTexture(0, "butterfly", _butterfly);

        for (int i = 0; i < 3; i++)
        {
            RenderTexture spectralTexture = i switch
            {
                0 => cascades[0].hk,
                1 => cascades[0].hk2,
                _ => cascades[0].nk
            };
            
            RenderTexture spectralTexture1 = i switch
            {
                0 => cascades[1].hk,
                1 => cascades[1].hk2,
                _ => cascades[1].nk
            };
            
            RenderTexture spectralTexture2 = i switch
            {
                0 => cascades[2].hk,
                1 => cascades[2].hk2,
                _ => cascades[2].nk
            };
            
            butterflyOperations.SetTexture(0, "pingPongIn", spectralTexture);
            butterflyOperations.SetTexture(0, "pingPongIn1", spectralTexture1);
            butterflyOperations.SetTexture(0, "pingPongIn2", spectralTexture2);

            int pingPong = 0;
            butterflyOperations.SetInt("direction", 0);
            for (int j = 0; j < (int)Mathf.Log(resolution, 2); j++)
            {
                butterflyOperations.SetInt("stage", j);
                butterflyOperations.SetInt("pingPong", pingPong);
                butterflyOperations.Dispatch(0, resolution, resolution, 1);
                pingPong = (pingPong + 1) % 2;
            }
        
            butterflyOperations.SetInt("direction", 1);
            for (int k = 0; k < (int)Mathf.Log(resolution, 2); k++)
            {
                butterflyOperations.SetInt("stage", k);
                butterflyOperations.SetInt("pingPong", pingPong);
                butterflyOperations.Dispatch(0, resolution, resolution, 1);
                pingPong = (pingPong + 1) % 2;
            }

            inversionAndPermutation.SetInt("pingPong", pingPong);
            inversionAndPermutation.SetInt("operationIndex", i);
            
            inversionAndPermutation.SetTexture(0, "heightMap", cascades[0].heightMap);
            inversionAndPermutation.SetTexture(0, "normalMap", cascades[0].normalMap);
            inversionAndPermutation.SetTexture(0, "pingPongOut", cascades[0].pingPong);
            inversionAndPermutation.SetTexture(0, "pingPongIn",  spectralTexture);
            inversionAndPermutation.Dispatch(0, resolution, resolution, 1);
        }
        
        foldingMapCompute.SetTexture(0, "heightMap", cascades[0].heightMap);
        foldingMapCompute.SetTexture(0, "foldingMap", cascades[0].foldingMap);
        foldingMapCompute.SetFloat("stepSize", lengthScale/resolution);
        foldingMapCompute.SetFloat("oceanResolution", resolution);
        foldingMapCompute.SetFloat("foamAmount", foamAmount);
        
        foldingMapCompute.Dispatch(0, resolution, resolution, 1);
    }

    //From stack overflow I think, just generates random numbers that follow a normal distribution
    private Vector2 NextGaussian() {
        float v1, v2, s;
        do {
            v1 = 2.0f * Random.Range(0f,1f) - 1.0f;
            v2 = 2.0f * Random.Range(0f,1f) - 1.0f;
            s = v1 * v1 + v2 * v2;
        } while (s >= 1.0f || s == 0f);

        s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);

        return  new Vector2(v1*s, v2*s);
    }

}
