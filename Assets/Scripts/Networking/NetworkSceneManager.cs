using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = NoMansLand.Scene.Scene;

public class NetworkSceneManager : Fusion.Behaviour
{
    private NetworkRunner _networkRunner;


    private void Awake()
    {
        _networkRunner = GetComponent<NetworkRunner>();
    }

    private void Start()
    {
        StartCoroutine(InitializeScene());
    }
    
    private IEnumerator InitializeScene()
    {
        Scene sceneObject = FindObjectOfType<NoMansLand.Scene.Scene>();
        
        sceneObject.Context.Runner = _networkRunner;
        sceneObject.Context.LocalPlayerRef = _networkRunner.LocalPlayer;
        sceneObject.Context.ObservedPlayerRef = _networkRunner.LocalPlayer;

        sceneObject.Initialize();

        yield return sceneObject.Activate();
    }
}

