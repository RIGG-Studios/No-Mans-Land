using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class ServerCreator : MonoBehaviour
{
    public static string LobbyName;

    [SerializeField] private TMP_InputField sessionNameInput;
    [SerializeField] private DisconnectionHandler disconnectionHandler;
    
    public void CreateSession()
    {
        string sessionName = sessionNameInput.text;

        if (sessionName == "")
        {
            disconnectionHandler.ShutdownCustomMessage("Invalid Server Name", "Please enter a valid server name");
            return;
        }
        
        GameLauncher.Instance.CreateRunner(GameMode.Host, sessionName, "MainScene");
    }
}
