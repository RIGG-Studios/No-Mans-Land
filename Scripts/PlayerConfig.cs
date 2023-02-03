using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerConfig : MonoBehaviour {
    string username = "";

    //Graphics settings
    int graphicsQuality = 1; //0 = low, 1 = medium, 2 = high
    int vfxQuality = 1; //0 = low, 1 = medium, 2 = high
    //Not sure if it would make more sense to store these as strings or an int. I'm going with the latter for now, but this can be changed later if necessary.

    //Audio settings
    float volumePercent = 100f;

    //Gameplay settings
    float mouseSens = 3f;


    //Initialization
    void Start(){
        LoadSettings();
    }

    //Loads current settings from Unity's PlayerPrefs. If the settings aren't there, uses the default value as a fallback.
    public void LoadSettings(){
        SetGraphicsQuality(PlayerPrefs.GetInt("Graphics Quality", 1));        
        SetVFXQuality(PlayerPrefs.GetInt("VFX Quality", 1));

        SetVolume(PlayerPrefs.GetFloat("Volume Percentage", 100f));

        SetSensitivity(PlayerPrefs.GetFloat("Mouse Sensitivity", 3f));
    }

    //Saves current settings to Unity's PlayerPrefs.
    public void SaveSettings(){
        PlayerPrefs.SetFloat("Volume Percentage", volumePercent);
        PlayerPrefs.SetFloat("Mouse Sensitivity", mouseSens);
    }

    //Clears all existing settings, then loads. Since there won't be anything in PlayerPrefs, this sets everything to its defaults.
    public void ResetSettings(){
        PlayerPrefs.DeleteAll();

        PlayerPrefs.SetString("Player Name", username);
        LoadSettings();
    }


    //BELOW THIS POINT: Setters and Getters for various options
    public void SetName(string newName){
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

    public void SetVolume(float newValue){
        volumePercent = newValue;
        AudioListener.volume = (volumePercent / 100);
    }
    public float GetVolume(){
        return volumePercent;
    }

    public void SetSensitivity(float newValue){
        mouseSens = newValue;
    }
    public float GetSensitivity(){
        return mouseSens;
    }

    public void SetGraphicsQuality(int newSetting){
        //check to make sure int is within proper range
        if(newSetting < 0 || newSetting > 2){
            Debug.Log("Invalid input");
        } else {
            graphicsQuality = newSetting;
        }
    }
    public int GetGraphicsQuality(){
        return graphicsQuality;
    }

    public void SetVFXQuality(int newSetting){
        //check to make sure int is within proper range
        if(newSetting < 0 || newSetting > 2){
            Debug.Log("Invalid input");
        } else {
            vfxQuality = newSetting;
        }
    }
    public int GetVFXQuality(){
        return vfxQuality;
    }
}