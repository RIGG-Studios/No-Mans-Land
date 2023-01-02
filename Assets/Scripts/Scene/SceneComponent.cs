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


    public void Init(Scene scene, SceneContext context)
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

    public void Activate()
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

    public void DeActivate()
    {
        if (_isActive == false)
        {
            return;
        }

        OnDeActivate();

        _isActive = false;
    }
    
    public void Tick()
    {
        if (_isActive == false)
        {
            return;
        }

        OnTick();
    }

    public void LateTick()
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
