using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class LobbySelectionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private LobbyTypes lobbyType;
    [SerializeField] private LobbySelection lobbySelection;

    public void OnPointerEnter(PointerEventData eventData)
    {
        lobbySelection.OnLobbyTypeSelected(lobbyType);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        lobbySelection.ResetUI();
    }
}
