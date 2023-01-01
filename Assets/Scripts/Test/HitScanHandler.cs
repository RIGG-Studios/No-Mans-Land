using Fusion;
using UnityEngine;

public class HitScanHandler 
{
    public static LagCompensatedHit RegisterHitScan(NetworkRunner runner, NetworkObject networkObject, Transform from,
        float length, LayerMask layers)
    {
         runner.LagCompensation.Raycast(from.position, from.forward, length,
            networkObject.InputAuthority, out var hitInfo, layers, HitOptions.IncludePhysX);

         return hitInfo;
    }
}
