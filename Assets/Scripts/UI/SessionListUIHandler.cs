using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;
using Unity.VisualScripting;
using System;

public class SessionListUIHandler : MonoBehaviour
{
    public TextMeshProUGUI statusText;

    public GameObject sessionItemListPrefab;

    public VerticalLayoutGroup verticalLayoutGroup;


    public void Clearlist()
    {
        //Delete children of vertical layout group for session list
        foreach (Transform child in verticalLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        //Hide Status message in list
        statusText.gameObject.SetActive(false); 
    }

    public void AddToList(SessionInfo sessionInfo)
    {
        //Add a new room to the list
        SessionInfoListUIHandler addedsessionInfoListUIItem = Instantiate(sessionItemListPrefab, verticalLayoutGroup.transform).GetComponent<SessionInfoListUIHandler>();
        
        addedsessionInfoListUIItem.SetInformation(sessionInfo);


        //Hook up events
        addedsessionInfoListUIItem.OnJoinSession += AddedSessionInfoListUIItem_OnJoinSession;
    }

    private void AddedSessionInfoListUIItem_OnJoinSession(SessionInfo obj)
    {
        throw new NotImplementedException();    
    }

    public void NoSessionsFound()
    {
        statusText.text = "No game session found";
        statusText.gameObject.SetActive(true);
    }
    
    public void OnLookingForGameSessions()
    {
        statusText.text = "Looking for game sessions";
        statusText.gameObject.SetActive(true);
    }
}
