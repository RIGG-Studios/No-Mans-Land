using Fusion;
using UnityEngine;
using UnityEngine.VFX;


[RequireComponent(typeof(AudioSource))]
public class CannonController : ContextBehaviour
{
    [Networked(OnChanged = nameof(OnAttackChanged), OnChangedTargets = OnChangedTargets.All)]
    public bool IsAttacking { get; set; }
    
    [Networked] 
    public NetworkBool isOccupied { get; set; }
    
    [Networked]
    private NetworkButtons ButtonsPrevious { get; set; }
    
    [Networked]
    private float pitch { get; set; }
    
    [Networked]
    private float yaw { get; set; }
    
    [Networked]
    private TickTimer fireCooldown { get; set; }
    
    [Networked]
    private NetworkPlayer OccupiedPlayer { get; set; }

    [SerializeField] private Projectile projectile;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Camera cannonCamera;
    [SerializeField] private VisualEffect muzzleFlash;
    [SerializeField] private Item ammoItem;
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private AudioClip[] fireSoundEffects;

    [SerializeField] private float cannonBallVelocity;
    [SerializeField] private float fireRate;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float xRotationMax;
    [SerializeField] private float yRotationMax;
    [SerializeField] private float originalRotation;
    [SerializeField] private float aimFOV = 60f;

    private AudioSource _audioSource;


    protected override void Awake()
    {
        base.Awake();
        _audioSource = GetComponent<AudioSource>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData input))
        {
            return;
        }

        float fov = input.IsAiming ? aimFOV : 75f;
        cannonCamera.fieldOfView = Mathf.Lerp(cannonCamera.fieldOfView, fov, Runner.DeltaTime * 5f);

        UpdateRotation(input);
        UpdateAttack(input);
    }

    public override void Spawned()
    {
        pitch = originalRotation;
    }

    private void UpdateAttack(NetworkInputData input)
    {
        if (!OccupiedPlayer.Inventory.FindItem(ammoItem.itemID, out ItemListData itemData) || !Object.HasStateAuthority)
        {
            return;
        }
        
        NetworkButtons pressed = input.Buttons.GetPressed(ButtonsPrevious);
        
        ButtonsPrevious = input.Buttons;

        if (pressed.IsSet(PlayerButtons.Fire) && fireCooldown.ExpiredOrNotRunning(Runner))
        {
            fireCooldown = TickTimer.CreateFromSeconds(Runner, fireRate);
            Projectile projectileInstance = Runner.Spawn(projectile, shootPoint.position + shootPoint.forward,
                Quaternion.LookRotation(shootPoint.forward), Object.InputAuthority);
            
           projectileInstance.Init(shootPoint,shootPoint.forward * cannonBallVelocity, 10f);
           IsAttacking = true;
           OccupiedPlayer.Inventory.UpdateItemStack(itemData, 1);

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
        _audioSource.PlayOneShot(fireSoundEffects[Random.Range(0, fireSoundEffects.Length)]);

        if (Object.HasInputAuthority)
        {
            cameraShake.ShakeCamera("Fire");
        }
    }
    
    
    private void UpdateRotation(NetworkInputData input)
    {
        yaw += input.RawLookY * rotationSpeed;
        pitch += input.RawLookX * rotationSpeed;

        pitch = Mathf.Clamp(pitch, originalRotation-xRotationMax, originalRotation+xRotationMax);
        yaw = Mathf.Clamp(yaw, -yRotationMax, yRotationMax);

        transform.localRotation = Quaternion.Euler(-yaw, pitch, 0.0f);
    }

    public void ToggleCamera(bool state) => cannonCamera.gameObject.SetActive(state);


    public void RequestOccupyCannon(bool occupy, PlayerRef requestedPlayer)
    {
        if (Object.HasStateAuthority)
        {
            isOccupied = occupy;
        
            if (occupy)
            {
                Object.AssignInputAuthority(requestedPlayer); 
            
                Context.Gameplay.TryFindPlayer(requestedPlayer, out Player player);

                if (player != null)
                {
                    OccupiedPlayer = player.ActivePlayer;
                }
            }
            else
            {
                Object.AssignInputAuthority(default);
                OccupiedPlayer = null;
            }
        }
    }


    public void OnSink()
    {
        if (isOccupied)
        {
            return;
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    private void RPC_RemoveInputAuthority()
    {
        NetworkPlayer.Local.Camera.Camera.gameObject.SetActive(true);
        cannonCamera.gameObject.SetActive(false);
    }
    
    public void Reset()
    {
        NetworkPlayer.Local.Camera.Camera.gameObject.SetActive(true);
        cannonCamera.gameObject.SetActive(false);
    }
}
