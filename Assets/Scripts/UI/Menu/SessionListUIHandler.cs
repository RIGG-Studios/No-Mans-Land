using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;
using Unity.VisualScripting;
using System;

public class SessionListUIHandler : UIComponent
{
    public GameObject sessionItemListPrefab;
    public Transform verticalLayoutGroup;


    public void Clearlist()
    {
        foreach (Transform child in verticalLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddToList(SessionInfo sessionInfo)
    {
        //Add a new room to the list
        SessionInfoListUIHandler addedsessionInfoListUIItem = Instantiate(sessionItemListPrefab, verticalLayoutGroup).GetComponent<SessionInfoListUIHandler>();
        
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
    //    statusText.text = "No game session found";
       // statusText.gameObject.SetActive(true);
    }
    
    public void OnLookingForGameSessions()
    {
 //      statusText.text = "Looking for game sessions";
    //    statusText.gameObject.SetActive(true);
    }
}
