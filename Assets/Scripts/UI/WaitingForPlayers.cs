using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingForPlayers : UIComponent
{
    [SerializeField] private Text playerCountText;
    
    private void Update()
    {
        int currentPlayers = Context.Session.Runner.SessionInfo.PlayerCount;
        int maxPlayers = Context.Session.Runner.SessionInfo.MaxPlayers;


        playerCountText.text = currentPlayers + "/" + maxPlayers;
    }
}
