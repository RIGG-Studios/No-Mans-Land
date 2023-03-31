using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConfig 
{
    public string username = "";

    //Graphics settings
    public int graphicsQuality;
    public int shadowQuality;
    public int waterQuality;
    public int antiAliasingQuality;
    public int ambientOcclusionQuality;
    public int volumetrics;

    //Audio settings
    public float masterVolumePercent;
    public float effectsVolumePercent;
    public float musicVolumePercent;

    public void SetName(string newName)
    {
        if(newName == "")
            return;

        username = newName;
        PlayerPrefs.SetString("Player Name", username);
    }

    public void SetMasterVolume(float newValue)
    {
        masterVolumePercent = newValue;
        AudioListener.volume = (masterVolumePercent / 100);
    }

    public void SetEffectsVolume(float newValue)
    {
        effectsVolumePercent = newValue;
        //Implement effects volume
    }

    public void SetMusicVolume(float newValue)
    {
        musicVolumePercent = newValue;
        //Implement music volume
    }

    public void SetGraphicsQuality(int newQuality)
    {
        graphicsQuality = newQuality;
        QualitySettings.SetQualityLevel(newQuality, true);
    }

    public void SetShadowQuality(int qualityIndex)
    {
        shadowQuality = qualityIndex;
        //Implement shadow quality
    }

    public void SetWaterQuality(int qualityIndex)
    {
        waterQuality = qualityIndex;
        //Implement water quality
    }

    public void SetAAQuality(int qualityIndex)
    {
        antiAliasingQuality = qualityIndex;
        //Implement AA quality
    }

    public void SetAOQuality(int qualityIndex)
    {
        ambientOcclusionQuality = qualityIndex;
        //Implement AO quality
    }

    public void SetVolumetricQuality(int qualityIndex)
    {
        volumetrics = qualityIndex;
        //Implement volumetrics
    }

    //Loads current settings from Unity's PlayerPrefs
    public void LoadSettings()
    {
        SetMasterVolume(PlayerPrefs.GetFloat("Master Volume", 100f));
        SetEffectsVolume(PlayerPrefs.GetFloat("Effects Volume", 100f));       
        SetMusicVolume(PlayerPrefs.GetFloat("Music Volume", 100f));

        SetName(PlayerPrefs.GetString("Player Name", "Player Name"));
        SetGraphicsQuality(PlayerPrefs.GetInt("Graphics Quality", 0));

        SetVolumetricQuality(PlayerPrefs.GetInt("Volumetrics", 0));
        SetShadowQuality(PlayerPrefs.GetInt("Shadow Quality", 0));
        SetWaterQuality(PlayerPrefs.GetInt("Water Quality", 0));
        SetAAQuality(PlayerPrefs.GetInt("AA Quality", 0));
        SetAOQuality(PlayerPrefs.GetInt("AO Quality", 0));
    }

    //Saves current settings to Unity's PlayerPrefs
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("Master Volume", masterVolumePercent);
        PlayerPrefs.SetFloat("Effects Volume", effectsVolumePercent);
        PlayerPrefs.SetFloat("Music Volume", musicVolumePercent);

        PlayerPrefs.SetInt("Graphics Quality", graphicsQuality);
        PlayerPrefs.SetInt("Shadow Quality", shadowQuality);

        PlayerPrefs.SetInt("Water Quality", waterQuality);
        PlayerPrefs.SetInt("AA Quality", antiAliasingQuality);
        PlayerPrefs.SetInt("AO Quality", ambientOcclusionQuality);
        PlayerPrefs.SetInt("Volumetrics", volumetrics);
    }

    //Resets all settings to default
    public void ResetSettings()
    {
        PlayerPrefs.DeleteAll();
        LoadSettings();
    }
}