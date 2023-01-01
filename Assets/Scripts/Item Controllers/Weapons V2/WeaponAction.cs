using System.Collections.Generic;
using Fusion;
using UnityEngine;


[DisallowMultipleComponent]
public class WeaponAction : NetworkBehaviour
{
    [SerializeField] private Transform barrelTransform;
    
    [Networked] private int LastFireTick { get; set; }

    private int _lastVisibleFireTick;

    private List<WeaponBaseComponent> _weaponComponents = new(16);

    public void Init(Weapon weapon, byte id)
    {
        _weaponComponents.Clear();

        WeaponBaseComponent[] weaponComponents = GetComponents<WeaponBaseComponent>();

        if (weaponComponents.Length > 0)
        {
            for (int i = 0; i < weaponComponents.Length; i++)
            {
                weaponComponents[i].Weapon = weapon;
                weaponComponents[i].WeaponActionID = id;
                
                _weaponComponents.Add(weaponComponents[i]);
            }
        }
    }

    public void ProcessInput(NetworkInputData data, bool isBusy)
    {
        WeaponDesires desires = default;

        for (int i = 0; i < _weaponComponents.Count; i++)
        {
            _weaponComponents[i].ProcessInput(data, ref desires, isBusy);
        }
        
        for (int i = 0; i < _weaponComponents.Count; i++)
        {
            _weaponComponents[i].OnFixedUpdate();
        }
    }

    public void DoRender()
    {
        WeaponDesires desires = default;

        desires.HasFired = _lastVisibleFireTick < LastFireTick && LastFireTick > Runner.Tick - (int)(Runner.Simulation.Config.TickRate * 0.5f);

        for (int j = 0; j < _weaponComponents.Count; j++)
        {
            _weaponComponents[j].OnRender(ref desires);
        }

        _lastVisibleFireTick = LastFireTick;
    }

    public bool IsBusy()
    {
        for (int i = 0; i < _weaponComponents.Count; i++)
        {
            if (_weaponComponents[i].IsBusy)
            {
                return true;
            }
        }

        return false;
    }

    public void HasFired()
    {
        LastFireTick = Runner.Tick;
    }

    public override void Spawned()
    {
        _lastVisibleFireTick = LastFireTick;
    }
}
