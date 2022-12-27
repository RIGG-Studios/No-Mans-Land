using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEditor;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    [SerializeField] private GameObject sessionGameObject;
    [SerializeField] private Transform grid;

    private List<Session> _sessions = new();


    private void OnEnable()
    {
        NetworkCallBackEvents.ListUpdated += UpdateSessionList;
    }

    private void OnDisable()
    {
        NetworkCallBackEvents.ListUpdated -= UpdateSessionList;
    }

    private void UpdateSessionList(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        for (int i = 0; i < _sessions.Count; i++)
        {
            Destroy(_sessions[i].gameObject);
        }
        
        _sessions.Clear();
        
        for (int i = 0; i < sessionList.Count; i++)
        {
            Session session = Instantiate(sessionGameObject, grid).GetComponent<Session>();
            
            session.Init(sessionList[i].Name);
        }
    }
}
