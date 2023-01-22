using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class ScenePostProcessing : SceneComponent
{
    [SerializeField] private Volume defaultVolume;
    [SerializeField] private Volume underWaterVolume;
    [SerializeField] private Volume telescopeVolume;


    public enum PostProcessingTypes
    {
        Default,
        UnderWater,
        Telescope
    }

    public void EnablePostProcessing(PostProcessingTypes type)
    {
        switch (type)
        {
            case PostProcessingTypes.Default:
                defaultVolume.gameObject.SetActive(true);
                return;
            
            case PostProcessingTypes.Telescope:
                telescopeVolume.gameObject.SetActive(true);
                return;
            
            case PostProcessingTypes.UnderWater:
                underWaterVolume.gameObject.SetActive(true);
                return;
        }
    }

    public void DisablePostProcessing(PostProcessingTypes type)
    {
        switch (type)
        {
            case PostProcessingTypes.Default:
                defaultVolume.gameObject.SetActive(false);
                return;
            
            case PostProcessingTypes.Telescope:
                telescopeVolume.gameObject.SetActive(false);
                return;
            
            case PostProcessingTypes.UnderWater:
                underWaterVolume.gameObject.SetActive(false);
                return;
        }
    }
}
