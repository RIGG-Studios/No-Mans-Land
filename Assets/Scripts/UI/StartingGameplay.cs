using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartingGameplay : UIComponent
{
    [SerializeField] private Text timeText;
    
    private void Update()
    {
        float? time = Context.Session.GetStartingGameTime();


        if (time != null)
        {
            timeText.text = ((int)time).ToString();
        }
    }
}
