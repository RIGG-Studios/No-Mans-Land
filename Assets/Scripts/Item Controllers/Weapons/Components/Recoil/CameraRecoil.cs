using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraRecoil : WeaponComponent, IRecoil
{
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilSmoothness;
    [SerializeField] private float maxRecoilY;

    private bool _isAiming;
    private bool _isRecoil;
    private int _recoilStep;

    private float _recoilY;
    private float _recoilX;
    
    public override void OnEnable()
    {
        base.OnEnable();
        
        Weapon.SetCameraRecoil(this);
    }

    public override void FixedUpdateNetwork(NetworkInputData input, ItemDesires desires)
    {
        if (desires.HasFired)
        {
            DoRecoil();
        }
    }
    
    public void DoRecoil()
    {
        _isRecoil = true;
        
        _recoilX = recoilX / (_isAiming ? 2 : 1); 
        _recoilY = recoilY / (_isAiming ? 2 : 1);
        _recoilY = Mathf.Clamp(_recoilY, 0, recoilY * (maxRecoilY - recoilY / maxRecoilY));
    }

    private void Update()
    {
        if ((_isRecoil) && (_recoilStep < recoilSmoothness))
        {
            float y = (1.0f * _recoilY) / recoilSmoothness;
            float x = 1.0f * Random.Range(-_recoilX, _recoilX) / recoilSmoothness;

            NetworkPlayer.Local.Camera.UpdateRecoil(x, y);
            _recoilStep += 1;
        }
        else
        {

            _isRecoil = false;
            _recoilStep = 0;
        }
    }

    public void OnAimStateChanged(bool state)
    {
        _isAiming = state;
    }
}
