using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject nameInput;
    [SerializeField] private GameObject connectionScreen;

    [SerializeField] private TextMeshProUGUI welcomeText;
    [SerializeField] private TMP_InputField nameText;

    [Header("Audio")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private TextMeshProUGUI masterVolumeText;
    [SerializeField] private Slider effectsVolumeSlider;
    [SerializeField] private TextMeshProUGUI effectsVolumeText;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private TextMeshProUGUI musicVolumeText;

    [Header("Graphics")]
    [SerializeField] private TMP_Dropdown graphicsQualityDropdown;
    [SerializeField] private TMP_Dropdown shadowQualityDropdown;
    [SerializeField] private TMP_Dropdown waterQualityDropdown;
    [SerializeField] private TMP_Dropdown AntiAliasingDropdown;
    [SerializeField] private TMP_Dropdown AmbientOcclusionDropdown;
    [SerializeField] private TMP_Dropdown VolumetricsDropdown;

    [SerializeField] private CanvasFader canvasFader;

    private PlayerConfig _config;
    private bool starting = true;

    private void Start()
    {
        _config = new PlayerConfig();
        _config.LoadSettings();
        
        masterVolumeText.text = _config.masterVolumePercent + "%";
        masterVolumeSlider.value = _config.masterVolumePercent;

        effectsVolumeText.text = _config.effectsVolumePercent + "%";
        effectsVolumeSlider.value = _config.effectsVolumePercent;

        musicVolumeText.text = _config.musicVolumePercent + "%";
        musicVolumeSlider.value = _config.musicVolumePercent;

        nameText.text = _config.username;
        graphicsQualityDropdown.value = _config.graphicsQuality;
        shadowQualityDropdown.value = _config.shadowQuality;
        waterQualityDropdown.value = _config.waterQuality;
        AntiAliasingDropdown.value = _config.antiAliasingQuality;
        AmbientOcclusionDropdown.value = _config.ambientOcclusionQuality;
        VolumetricsDropdown.value = _config.volumetrics;

        starting = false;
    }

    private void PromptName()
    {
        mainMenu.SetActive(false);
        nameInput.SetActive(true);
    }

    public void OnLobbyConnection()
    {
        connectionScreen.SetActive(false);
        canvasFader.FadeIn();
        Invoke(nameof(ShowMainMenu), 1.0f);
    }

    public void SetMasterVolume(float value)
    {
        masterVolumeText.text = value + "%";
        _config.SetMasterVolume(value);
    }

    public void SetEffectsVolume(float value)
    {
        effectsVolumeText.text = value + "%";
        _config.SetEffectsVolume(value);
    }

    public void SetMusicVolume(float value)
    {
        musicVolumeText.text = value + "%";
        _config.SetMusicVolume(value);
    }

    public void SetGraphicsQuality(int qualityIndex)
    {
        if(!starting)
        {
            shadowQualityDropdown.value = qualityIndex;
            waterQualityDropdown.value = qualityIndex;
            AntiAliasingDropdown.value = qualityIndex;
        }

        if(qualityIndex == 0)
        {
            if(!starting)
            {
                AmbientOcclusionDropdown.value = 0;
                VolumetricsDropdown.value = 0;
            }
        }
        else 
        {
            if(!starting)
            {
                AmbientOcclusionDropdown.value = 1;
            }
        }

        if(qualityIndex == 2)
        {
            if(!starting)
            {
                VolumetricsDropdown.value = 3;
            }
        }

        _config.SetGraphicsQuality(qualityIndex);
    }

    public void SetShadowQuality(int qualityIndex) => _config.SetShadowQuality(qualityIndex);
    public void SetWaterQuality(int qualityIndex) => _config.SetWaterQuality(qualityIndex);
    public void SetAAQuality(int qualityIndex) => _config.SetAAQuality(qualityIndex);
    public void SetAOQuality(int qualityIndex) => _config.SetAOQuality(qualityIndex);
    public void SetVolumetricQuality(int qualityIndex) => _config.SetVolumetricQuality(qualityIndex);

    public void SetName(string name) => _config.SetName(name);

    public void ApplySettings() => _config.SaveSettings(); 
    public void ResetSettings() => _config.ResetSettings();

    private void ShowMainMenu()
    {
        canvasFader.FadeOut();
        mainMenu.SetActive(true);
        CheckName();
    }

    private void CheckName()
    {
        if(ClientInfo.ClientName == null)
        {
            PromptName();
            return;
        }
        
        welcomeText.text = "Welcome, " + ClientInfo.ClientName;
    }

}
