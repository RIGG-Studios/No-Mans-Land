using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using Fusion;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

public class CannonController : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnAttackChanged))]
    public bool IsAttacking { get; set; }
    
    [Networked, HideInInspector]
    public int CurrentAmmo { get; set; }
    
    [Networked] 
    public NetworkBool isOccupied { get; set; }
    
    [Networked, HideInInspector]
    public float NextFire { get; set; }
    
    [Networked]
    private NetworkButtons ButtonsPrevious { get; set; }

    [Networked]
    private float pitch { get; set; }
    
    [Networked]
    private float yaw { get; set; }

    [SerializeField] private Projectile projectile;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Camera cannonCamera;
    [SerializeField] private VisualEffect muzzleFlash;
    [SerializeField] private float cannonBallVelocity;
    [SerializeField] private float fireRate;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float xRotationMax;
    [SerializeField] private float yRotationMax;

    
    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData input))
        {
            return;
        }

        if (Object.HasInputAuthority)
        {
            NetworkPlayer.Local.Camera.Camera.gameObject.SetActive(false);
            cannonCamera.gameObject.SetActive(true);
        }

        UpdateRotation(input);
        UpdateAttack(input);
    }

    private void UpdateAttack(NetworkInputData input)
    {
        NetworkButtons pressed = input.Buttons.GetPressed(ButtonsPrevious);
        
        ButtonsPrevious = input.Buttons;

        if (pressed.IsSet(PlayerButtons.Fire))
        {
            NextFire = Runner.DeltaTime + 1f / fireRate;

           Projectile projectileInstance = Runner.Spawn(projectile, shootPoint.position + shootPoint.forward,
                Quaternion.LookRotation(shootPoint.forward), Object.InputAuthority);
           
           projectileInstance.Init(shootPoint,shootPoint.forward * cannonBallVelocity, 10f);
           IsAttacking = true;
           Invoke(nameof(ResetAttack), 0.09f);
        }
    }
    
    private void ResetAttack() => IsAttacking = false;

    
    private static void OnAttackChanged(Changed<CannonController> changed)
    {
        bool isAttackingNow = changed.Behaviour.IsAttacking;
        
        changed.LoadOld();

        bool isAttackingPrevious = changed.Behaviour.IsAttacking;

        if (isAttackingNow && !isAttackingPrevious)
        {
            changed.Behaviour.OnAttackRemote();
        }
    }
    
    

    private void OnAttackRemote()
    {
        muzzleFlash.Play();
    }
    
    
    private void UpdateRotation(NetworkInputData input)
    {
        yaw += input.RawLookY * rotationSpeed;
        pitch += input.RawLookX * rotationSpeed;

        pitch = Mathf.Clamp(pitch, -xRotationMax, xRotationMax);
        yaw = Mathf.Clamp(yaw, -yRotationMax, yRotationMax);

        transform.localRotation = Quaternion.Euler(-yaw, pitch, 0.0f);
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestOccupyCannon(bool occupy, PlayerRef requestedPlayer = default)
    {
        isOccupied = occupy;

        if (requestedPlayer.IsValid)
        {
            Object.AssignInputAuthority(requestedPlayer);
        }
        else
        {
            Object.AssignInputAuthority(default);
        }
    }

    public void Reset()
    {
        NetworkPlayer.Local.Camera.Camera.gameObject.SetActive(true);
        cannonCamera.gameObject.SetActive(false);
    }
}
