using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Matchmaking : NetworkBehaviour
{
    [SerializeField] private GameObject matchFoundUI;

    private int _playersReadyCount;

    private bool _localPlayerJoined;

    private NetworkRunner _networkRunner;

    public void OnEnable()
    {
        NetworkCallBackEvents.onPlayerJoined += OnPlayerJoined;
    }

    public void OnDisable()
    {
        NetworkCallBackEvents.onPlayerJoined -= OnPlayerJoined;
    }

    public void Update()
    {
        if (_localPlayerJoined)
        {

        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        _localPlayerJoined = true;

        if (!Object.HasStateAuthority)
        {
            return;
        }

        if (runner.SessionInfo.PlayerCount >= runner.SessionInfo.MaxPlayers)
        {
            RPC_MatchFound();
        }
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    private void RPC_MatchFound()
    {
        matchFoundUI.SetActive(true);
    }

    public void ReadyUp()
    {
        RPC_ReadyUp();
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    private void RPC_ReadyUp()
    {
        _playersReadyCount++;

        if (_playersReadyCount >= Runner.SessionInfo.PlayerCount)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        Debug.Log("Start Game!");
        
    }
        
    }
