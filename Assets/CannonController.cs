using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using Fusion;
using Unity.Mathematics;
using UnityEngine;

public class CannonController : NetworkBehaviour
{
    [Networked]
    public int CurrentAmmo { get; set; }
    
    [Networked] 
    public NetworkBool isOccupied { get; set; }
    
    [Networked, HideInInspector]
    public Vector2 LookRotation { get; set; }
    
    [Networked]
    public float NextFire { get; set; }

    [SerializeField] private Projectile projectile;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Camera cannonCamera;
    [SerializeField] private float cannonBallVelocity;
    [SerializeField] private float fireRate;
    [SerializeField] private float smoothing;
    [SerializeField] private float rotationSpeed;
    
    private Quaternion _nativeRotation;
    
    [Networked]
    private NetworkButtons ButtonsPrevious { get; set; }
    

    public override void Spawned()
    {
        _nativeRotation.eulerAngles = new Vector3(0f, LookRotation.y, 0f);
    }

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
        NetworkButtons released = input.Buttons.GetReleased(ButtonsPrevious);
        
        ButtonsPrevious = input.Buttons;

        if (pressed.IsSet(PlayerButtons.Fire))
        {
            NextFire = Runner.DeltaTime + 1f / fireRate;

           Projectile projectileInstance = Runner.Spawn(projectile, shootPoint.position + shootPoint.forward,
                Quaternion.LookRotation(shootPoint.forward), Object.InputAuthority);
           
           projectileInstance.Init(shootPoint,shootPoint.forward * cannonBallVelocity, 10f);
        }
    }
    

    private void UpdateRotation(NetworkInputData input)
    {
        LookRotation = new Vector2(input.RawLookX, input.RawLookY) * rotationSpeed;
        
        Quaternion camTargetRotation = _nativeRotation * Quaternion.AngleAxis(LookRotation.y + (0), Vector3.left);
        Quaternion bodyTargetRotation = _nativeRotation * Quaternion.AngleAxis(LookRotation.x + (0), Vector3.up);
        
        transform.rotation =  Quaternion.Slerp(transform.localRotation, camTargetRotation * bodyTargetRotation, smoothing);

        Quaternion old = transform.rotation;
        old.z = 0.0f;

        transform.rotation = old;
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
