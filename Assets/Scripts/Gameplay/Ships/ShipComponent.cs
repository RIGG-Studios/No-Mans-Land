using UnityEngine;

public class ShipComponent : MonoBehaviour
{
    protected Ship Ship;

    public void Init(Ship ship)
    {
        Ship = ship;
    }
}
