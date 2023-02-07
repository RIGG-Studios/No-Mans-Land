using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbySelection : UIComponent
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;


    public void OnLobbyTypeSelected(LobbyTypes lobby)
    {
        nameText.text = lobby.name;
        descriptionText.text = lobby.description;
    }

    public void ResetUI()
    {
        nameText.text = "";
        descriptionText.text = "";
    }
}
