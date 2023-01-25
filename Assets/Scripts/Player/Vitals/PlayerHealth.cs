using System;
using Fusion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class PlayerHealth : NetworkHealthHandler, INetworkDamagable
{
    [Networked(OnChanged = nameof(OnDeadChanged))]
    public NetworkBool IsDead { get; private set; }

    [Networked]
    public float Oxygen { get; private set; }

    [SerializeField] private Text healthText;
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject[] disableOnDeath;

    [Space]
    
    [SerializeField] private Slider oxygenSlider;
    [SerializeField, Range(0, 10)] private float oxygenDecayRate;
    
    public UnityEvent onDeath;
    
    private const byte StartingHealth = 100;


    public INetworkDamagable.DamageTypes Type => INetworkDamagable.DamageTypes.Player;
    public NetworkPlayer Owner { get; set; }

    private void Start()
    {
        Owner = GetComponent<NetworkPlayer>();
    }

    public override void Spawned()
    {
        if (!Object.HasStateAuthority)
        {
            return;
        }
        
        Health = StartingHealth;
        Oxygen = 1f;
    }
    

    private static void OnDeadChanged(Changed<PlayerHealth> changed)
    {
        bool isDeadCurrent = changed.Behaviour.IsDead;

        changed.LoadOld();
        
        if (isDeadCurrent)
            changed.Behaviour.OnDeath();
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority)
        {
            healthText.text = string.Format("<color={0}>+</color> {1}", "red", Health);
        }
    }
    
    private void OnDeath()
    {
        if (Object.HasStateAuthority)
        {
            Context.Gameplay.OnPlayerDeath(this);
        }
        
        model.gameObject.SetActive(false);

        if (Object.HasInputAuthority)
        {
            deathPanel.SetActive(true);
            onDeath?.Invoke();

            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                disableOnDeath[i].SetActive(false);
            }

            NetworkPlayer.Local.Camera.Fall();
        }
    }

    public override bool Damage(ref HitData hitData)
    {
        Health -= (byte)hitData.Damage;

        if (Health <= 0)
        {
            hitData.IsFatal = true;

            if (Object.HasStateAuthority)
            {
                Health = 0;
                IsDead = true;
            }
        }
        
        return true;
    }

    public override bool Heal(float amount)
    {
        Debug.Log($"{Time.time} {transform.name} got healed, got {Health} left ");
        
        Health += (byte)amount;
        
        if (Health > 100)
        {
            Health = 100;
        }
        return true;
    }


    protected override void OnHealthReduced()
    {
        if (Object.HasInputAuthority)
        {
            NetworkPlayer.Local.UI.EnableMenu("DamageMenu");
        }
    }


    public bool ProcessHit(ref HitData hit)
    {
        return Damage(ref hit);
    }

    public void HitPerformed(HitData hit)
    {
        
    }
}
