using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{
    string name = "";
    float volumeLevel = 1.0f;
    float mouseSens = 5.0f;

    void Start(){
        //loadConfig();
    }

    public void setName(string newName){
        name = newName;
    }

    //closes the game (does not work in editor, but should work in an actual build)
    public void ExitButton(){
        Application.Quit();
    }
}
