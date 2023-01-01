using UnityEngine;
using UnityEngine.InputSystem.DualShock;

public static class SceneShipHandlerInstance
{
    private static SceneShipHandler _shipHandler;


    public static void InitShipHandler(SceneShipHandler shipHandler)
    {
        if (_shipHandler != null)
        {
            _shipHandler.DeActivate();
        }

        _shipHandler = shipHandler;
        _shipHandler.Activate();
    }


    public static SceneShipHandler GetShipHandler()
    {
        if (_shipHandler == null)
        {
            Debug.Log("Ship Handler hasn't been setup yet");
            return null;
        }

        return _shipHandler;
    }
}
