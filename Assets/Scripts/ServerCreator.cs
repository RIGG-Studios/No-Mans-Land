using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class ServerCreator : MonoBehaviour
{
    public static string LobbyName;

    [SerializeField] private TMP_InputField sessionNameInput;

    public void CreateSession()
    {
        string sessionName = sessionNameInput.text;
        
        Debug.Log(sessionName);
        GameLauncher.Instance.CreateRunner(GameMode.Host, sessionName, "MainScene");
    }
}
