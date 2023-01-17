using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{

    //closes the game (does not work in editor, but should work in an actual build)
    public void ExitButton(){
        Application.Quit();
    }
}
