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
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI volumeText;
    [SerializeField] private TMP_InputField nameText;


    [SerializeField] private CanvasFader canvasFader;

    private PlayerConfig _config;

    private void Start()
    {
        _config = new PlayerConfig();
        _config.LoadSettings();
        
        volumeText.text = _config.volumePercent + "%";
        volumeSlider.value = _config.volumePercent;
        nameText.text = _config.username;
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

    public void SetVolume(float value)
    {
        volumeText.text = value + "%";
        _config.SetVolume(value);
    }

    public void SetName(string name)
    {
        _config.SetName(name);
    }

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
