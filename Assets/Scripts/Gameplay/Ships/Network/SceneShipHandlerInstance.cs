using UnityEngine;
using UnityEngine.InputSystem.DualShock;

public static class SceneShipHandlerInstance
{
    public static SceneShipHandler ShipHandler;


    public static void InitShipHandler(SceneShipHandler shipHandler)
    {
        ShipHandler = shipHandler;
    }
}
