using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class WeaponRecoil : WeaponComponent, IRecoil
{
    [SerializeField] private Transform recoilPositionTransform;
    [SerializeField] private Transform recoilRotationTransform;
    [SerializeField] private float dampTime;
    [SerializeField] private float recoil1 = 35;
    [SerializeField] private float recoil2 = 50;
    [SerializeField] private float recoil3 = 35;
    [SerializeField] private float recoil4 = 50;

    [SerializeField] private Vector3 recoilPosition;
    [SerializeField] private Vector3 recoilRotation;
    
    
    private Vector3 _recoil1Pos;
    private Vector3 _recoil2Pos;
    private Vector3 _recoil3Pos;
    private Vector3 _recoil4Pos;

    private bool _aiming;
    
    public override void OnEnable()
    {
        base.OnEnable();
        
        Weapon.SetWeaponRecoil(this);
    }
    

    public override void FixedUpdateNetwork(WeaponContext context, ItemDesires desires)
    {
        if (Object.HasInputAuthority && desires.HasFired && Runner.IsForward)
        {
            DoRecoil();
        }
    }

    public void DoRecoil()
    {
        _recoil1Pos += new Vector3(recoilRotation.x, Random.Range(-recoilRotation.y, recoilRotation.y), 
            Random.Range(-recoilRotation.z, recoilRotation.z));

        _recoil3Pos += new Vector3(Random.Range(-recoilPosition.x, recoilPosition.x), 
            Random.Range(-recoilPosition.y, recoilPosition.y), recoilPosition.z);

        if (_aiming)
        {
            _recoil1Pos /= 3.5f;
            _recoil3Pos /= 3.5f;
        }
    }
    
    private void Update()
    {
        _recoil1Pos = Vector3.Lerp(_recoil1Pos, Vector3.zero, recoil1 * Time.deltaTime);
        _recoil2Pos = Vector3.Lerp(_recoil2Pos, _recoil1Pos, recoil2 * Time.deltaTime);
        _recoil3Pos = Vector3.Lerp(_recoil3Pos, Vector3.zero, recoil3 * Time.deltaTime);
        _recoil4Pos = Vector3.Lerp(_recoil4Pos, _recoil3Pos, recoil4 * Time.deltaTime);

        recoilPositionTransform.localPosition =
            Vector3.Lerp(recoilPositionTransform.localPosition, _recoil3Pos, dampTime);

        recoilRotationTransform.localRotation = Quaternion.Slerp(recoilRotationTransform.localRotation,
            Quaternion.Euler(_recoil1Pos), dampTime);
    }

    public void OnAimStateChanged(bool isAiming)
    {
        _aiming = isAiming;
    }
}
