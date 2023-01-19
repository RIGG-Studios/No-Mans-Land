using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ScenePostProcessing : SceneComponent
{
    [SerializeField] private Volume defaultVolume;
    [SerializeField] private Volume underWaterVolume;


    public enum PostProcessingTypes
    {
        Default,
        UnderWater
    }

    public void EnablePostProcessing(PostProcessingTypes type)
    {
        if (type == PostProcessingTypes.Default)
        {
            defaultVolume.gameObject.SetActive(true);
            underWaterVolume.gameObject.SetActive(false);
        }

        if (type == PostProcessingTypes.UnderWater)
        {
            defaultVolume.gameObject.SetActive(false);
            underWaterVolume.gameObject.SetActive(true);
        }
    }
}
