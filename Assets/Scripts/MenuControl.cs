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

    //Options and config data
    private PlayerConfig config;
    [SerializeField] private TMP_Text volumeText;

    void Start(){
        config = optionsManager.GetComponent<PlayerConfig>();

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
        volumeText.text = (config.GetVolume()) + "%";
    }

    //closes the game (does not work in editor, but should work in an actual build)
    public void ExitButton(){
        Application.Quit();
    }
}