using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConfig : MonoBehaviour {
    string username = "";

    //Graphics settings

    //Audio settings
    float volumePercent = 100f;

    //Gameplay settings
    float mouseSens = 3f;

    void Start(){
        LoadSettings();
    }

    public void SetName(string newName)
    {
        if(newName == ""){
            Debug.Log("Invalid username.");
        } else {
            username = newName;
            Debug.Log("Name set to " + username + ".");
            PlayerPrefs.SetString("Player Name", username);
        }
    }
    public string GetName(){
        return username;
    }

    public void SetVolume(float newValue)
    {
        volumePercent = newValue;
        AudioListener.volume = (volumePercent / 100);
    }
    public float GetVolume()
    {
        return volumePercent;
    }

    public void SetSensitivity(float newValue)
    {
        mouseSens = newValue;
    }
    public float GetSensitivity(){
        return mouseSens;
    }

    //Loads current settings from Unity's PlayerPrefs
    public void LoadSettings()
    {
        SetVolume(PlayerPrefs.GetFloat("Volume Percentage", 100f));
        SetSensitivity(PlayerPrefs.GetFloat("Mouse Sensitivity", 3f));
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