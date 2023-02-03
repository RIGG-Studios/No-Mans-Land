using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuControl : MonoBehaviour {
    public TMP_InputField usernameInput;

    [SerializeField] private GameObject optionsManager;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject nameInput;

    bool playerInQueue = false;

    //Options and config data
    private PlayerConfig config;
    [SerializeField] private TMP_Dropdown graphicsPicker = null;
    [SerializeField] private TMP_Dropdown vfxPicker = null;

    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private TMP_Text volumeText;

    [SerializeField] private Slider sensitivitySlider = null;
    [SerializeField] private TMP_Text sensText;

    void Start(){
        config = optionsManager.GetComponent<PlayerConfig>();

        //If the player hasn't set a name, asks them to input one.
        if(config.GetName() == ""){
            PromptName();
        }
    }

    void Update(){
        if(optionsMenu.activeInHierarchy == true){
            UpdateOptionsText();
        }
    }

    void PromptName(){
        mainMenu.SetActive(false);
        nameInput.SetActive(true);
    }

    public void InitializeName(){
        config.SetName(usernameInput.text);
        if(config.GetName() != ""){
            mainMenu.SetActive(true);
            nameInput.SetActive(false);
        }
    }

    void UpdateOptionsText(){
        //graphicsPicker.value = config.GetGraphicsQuality();
        //vfxPicker.value = config.GetVFXQuality();

        volumeSlider.value = config.GetVolume();
        volumeText.text = config.GetVolume() + "%";

        //sensitivitySlider.value = config.GetSensitivity();
        //sensText.text = config.GetSensitivity() + "";
    }

    //closes the game (does not work in editor, but should work in an actual build)
    public void ExitButton(){
        Application.Quit();
    }

    //Puts the player into the quickplay queue
    public void JoinQueue(){
        playerInQueue = true;
    }
    //Removes the player from the quickplay queue
    public void LeaveQueue(){
        playerInQueue = false;
    }
    public bool GetQueueStatus(){
        return playerInQueue;
    }
}