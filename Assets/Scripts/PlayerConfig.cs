using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConfig 
{
    public string username = "";

    //Graphics settings

    //Audio settings
    public float volumePercent = 100f;

    //Gameplay settings
    public float mouseSens = 3f;

    public void SetName(string newName)
    {
        if(newName == "")
            return;

        username = newName;
        PlayerPrefs.SetString("Player Name", username);
    }

    public void SetVolume(float newValue)
    {
        volumePercent = newValue;
        AudioListener.volume = (volumePercent / 100);
    }

    public void SetSensitivity(float newValue)
    {
        mouseSens = newValue;
    }

    //Loads current settings from Unity's PlayerPrefs
    public void LoadSettings()
    {
        SetVolume(PlayerPrefs.GetFloat("Volume Percentage", 100f));
        SetSensitivity(PlayerPrefs.GetFloat("Mouse Sensitivity", 3f));
        SetName(PlayerPrefs.GetString("Player Name", "Player Name"));
    }

    //Saves current settings to Unity's PlayerPrefs
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("Volume Percentage", volumePercent);
        PlayerPrefs.SetFloat("Mouse Sensitivity", mouseSens);
    }

    //Resets all settings to default
    public void ResetSettings()
    {
        PlayerPrefs.DeleteAll();
        LoadSettings();
    }
}