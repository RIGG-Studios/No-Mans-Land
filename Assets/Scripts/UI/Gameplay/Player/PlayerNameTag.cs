using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameTag : SimulationBehaviour
{
    [SerializeField] private Image friendlyPlayerIcon;
    [SerializeField] private NetworkPlayer player;
    [SerializeField] private TextMeshProUGUI playerNameText;


    private void Update()
    {
        if (player.Health.IsDead)
        {
            friendlyPlayerIcon.gameObject.SetActive(false);
            playerNameText.gameObject.SetActive(false);
            return;
        }
        
        if (Object.HasInputAuthority)
        {
            friendlyPlayerIcon.enabled = false;
            playerNameText.enabled = false;
            return;
        }

        if (NetworkPlayer.Local == null)
        {
            return;
        }

        transform.LookAt(NetworkPlayer.Local.Camera.transform);
        
        playerNameText.text = player.Owner.PlayerName.ToString();

        bool isFriendly = player.Owner.Stats.TeamID == NetworkPlayer.Local.Owner.Stats.TeamID;
        friendlyPlayerIcon.color = isFriendly ? Color.green : Color.red;
    }
}
