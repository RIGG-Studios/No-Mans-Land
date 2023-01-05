using UnityEngine;

public interface IReloader
{
    int CurrentAmmo { get; }
    int ReserveAmmo { get; }

    bool IsReloading { get; }


    void OnFired();
    void DecrementCurrentAmmo(int amount = 1);
    void IncrementCurrentAmmo(int amount = 1);
    
    void Reload();
}
