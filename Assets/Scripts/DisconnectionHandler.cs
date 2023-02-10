using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEditor;
using UnityEngine;

public class DisconnectionHandler : MonoBehaviour
{
    [SerializeField] private GameObject disconnectUI;
    [SerializeField] private TextMeshProUGUI disconnectionStatusText;
    [SerializeField] private TextMeshProUGUI disconnectionReasonText;

    private void Start()
    {
        Close();
    }
    
    public void ShutdownCustomMessage(string status, string message)
    {
        disconnectionStatusText.text = status;
        disconnectionReasonText.text = message;
        disconnectUI.SetActive(true);
    }

    public void OnShutdown(ShutdownReason reason)
    {
        if (disconnectUI == null)
        {
            return;
        }
        
        (string status, string message) = ShutDownReason(reason);

        disconnectionStatusText.text = status;
        disconnectionReasonText.text = message;
        disconnectUI.SetActive(true);
    }

    public void OnConnectionFailed(NetConnectFailedReason reason)
    {
        if (disconnectUI == null)
        {
            return;
        }

        
        (string status, string message) = ConnectFailedReason(reason);
        
        disconnectionStatusText.text = status;
        disconnectionReasonText.text = message;
        disconnectUI.SetActive(true);
    }

    public void Close()
    {
        disconnectUI.SetActive(false);
    }
    
    private static (string, string) ShutDownReason(ShutdownReason reason)
    {
        switch (reason)
        {
            case ShutdownReason.Ok:
                return (null, null);
            case ShutdownReason.Error:
                return ("Error", "Shutdown was caused by some internal error");
            case ShutdownReason.IncompatibleConfiguration:
                return ("Incompatible Config", "Mismatching type between client Server Mode and Shared Mode");
            case ShutdownReason.ServerInRoom:
                return ("Room name in use", "There's a room with that name! Please try a different name or wait a while.");
            case ShutdownReason.DisconnectedByPluginLogic:
                return ("Disconnected By Plugin Logic", "You were kicked, the room may have been closed");
            case ShutdownReason.GameClosed:
                return ("Game Closed", "The session cannot be joined, the game is closed");
            case ShutdownReason.GameNotFound:
                return ("Game Not Found", "This room does not exist");
            case ShutdownReason.MaxCcuReached:
                return ("Max Players", "The Max CCU has been reached, please try again later");
            case ShutdownReason.InvalidRegion:
                return ("Invalid Region", "The currently selected region is invalid");
            case ShutdownReason.GameIdAlreadyExists:
                return ("ID already exists", "A room with this name has already been created");
            case ShutdownReason.GameIsFull:
                return ("Game is full", "This lobby is full!");
            case ShutdownReason.InvalidAuthentication:
                return ("Invalid Authentication", "The Authentication values are invalid");
            case ShutdownReason.CustomAuthenticationFailed:
                return ("Authentication Failed", "Custom authentication has failed");
            case ShutdownReason.AuthenticationTicketExpired:
                return ("Authentication Expired", "The authentication ticket has expired");
            case ShutdownReason.PhotonCloudTimeout:
                return ("Cloud Timeout", "Connection with the Photon Cloud has timed out");
            default:
                Debug.LogWarning($"Unknown ShutdownReason {reason}");
                return ("Unknown Shutdown Reason", $"{(int)reason}");
        }
    }


    private static (string, string) ConnectFailedReason(NetConnectFailedReason reason)
    {
        switch (reason)
        {
            case NetConnectFailedReason.Timeout:
                return ("Timed Out", "");
            case NetConnectFailedReason.ServerRefused:
                return ("Connection Refused", "The lobby may be currently in-game");
            case NetConnectFailedReason.ServerFull:
                return ("Server Full", "");
            default:
                Debug.LogWarning($"Unknown NetConnectFailedReason {reason}");
                return ("Unknown Connection Failure", $"{(int)reason}");
        }
    }
}
