using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuControl : MonoBehaviour 
{
   // public TMP_InputField usernameInput;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject nameInput;
    [SerializeField] private GameObject connectionScreen;
    [SerializeField] private TextMeshProUGUI welcomeText;
    [SerializeField] private CanvasFader canvasFader;

    
    //Options and config data
    private PlayerConfig config;
    [SerializeField] private TMP_Text volumeText;
    

    void CheckName()
    {
        if(ClientInfo.ClientName == null)
        {
            PromptName();
            return;
        }
        
        welcomeText.text = "Welcome, " + ClientInfo.ClientName;
    }

    void Update()
    {
        if(optionsMenu.activeInHierarchy == true)
        {
            UpdateOptionsText();
        }
    }

    void PromptName()
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

    private void ShowMainMenu()
    {
        canvasFader.FadeOut();
        mainMenu.SetActive(true);
        CheckName();
    }

    void UpdateOptionsText()
    {
        volumeText.text = (config.GetVolume()) + "%";
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}