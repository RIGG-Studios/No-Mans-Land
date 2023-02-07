using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : UIComponent
{
    [SerializeField] private Text gameplayTimeText;


    private void Update()
    {
        float time = Context.Session.GameplayTimer;
        
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        
        gameplayTimeText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}
