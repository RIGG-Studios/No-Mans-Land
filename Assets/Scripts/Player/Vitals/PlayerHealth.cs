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

    [SerializeField] private Text healthText;
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject[] disableOnDeath;
    
    public UnityEvent<HitData> onDamageTaken;
    public UnityEvent onDeath;
    
    private const byte StartingHealth = 100;
    
    
    public NetworkPlayer Owner { get; set; }

    private void Start()
    {
        Owner = GetComponent<NetworkPlayer>();
        Health = StartingHealth;
    }

    
    private static void OnDeadChanged(Changed<PlayerHealth> changed)
    {
        bool isDeadCurrent = changed.Behaviour.IsDead;

        changed.LoadOld();
        
        if (isDeadCurrent)
            changed.Behaviour.OnDeath();
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
        Debug.Log($"{Time.time} {transform.name} took damage got {Health} left ");

        if (Health <= 0)
        {
            hitData.IsFatal = true;
            Debug.Log($"{Time.time} {transform.name} died");
            Health = 0;
            IsDead = true;
        }
        
        return true;
    }

    public override bool Heal(float amount)
    {
        RPC_Heal(amount);

        return true;
    }


    protected override void OnHealthReduced()
    {
        if (Object.HasInputAuthority)
        {
            NetworkPlayer.Local.UI.EnableMenu("DamageMenu");
            healthText.text = string.Format("<color={0}>+</color> {1}", "red", Health - 10f);
        }
    }
    
    
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_Heal(float healAmount)
    {
        Debug.Log($"{Time.time} {transform.name} got healed, got {Health} left ");
        Health += (byte)healAmount;
        
        if (Health > 100)
        {
            Health = 100;
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
