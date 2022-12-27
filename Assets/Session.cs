using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Session : MonoBehaviour
{
    private string roomName;

    private NetworkRunner _runner;


    public void Init(string rName)
    {
        roomName = rName;
    }

    public void JoinGame()
    {
      //  _runner.StartGame();
    }
}
