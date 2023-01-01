using System.Collections;
using System.Collections.Generic;
using NoMansLand.Scene;
using UnityEngine;

public class SceneComponent : MonoBehaviour
{
    public SceneContext Context => _context;
    public Scene Scene => _scene;
    public bool IsActive => _isActive;
    public bool IsInitialized => _isInitialized;

    private Scene _scene;
    private SceneContext _context;
    private bool _isInitialized;
    private bool _isActive;


    internal void Init(Scene scene, SceneContext context)
    {
        if (_isInitialized)
        {
            return;
        }

        _scene = scene;
        _context = context;
        
        OnInit();

        _isInitialized = true;
    }

    internal void Activate()
    {
        if (!_isInitialized)
        {
            return;
        }

        if (_isActive)
        {
            return;
        }
        
        OnActivate();
        _isActive = true;
    }

    internal void DeActivate()
    {
        if (_isActive == false)
        {
            return;
        }

        OnDeActivate();

        _isActive = false;
    }
    
    internal void Tick()
    {
        if (_isActive == false)
        {
            return;
        }

        OnTick();
    }

    internal void LateTick()
    {
        if (_isActive == false)
        {
            return;
        }

        OnLateTick();
    }

    
    protected virtual void OnDeActivate() { }
    protected virtual void OnActivate() { }
    protected virtual void OnInit() { }
    protected virtual void OnTick() { }
    protected virtual void OnLateTick() { }
}
