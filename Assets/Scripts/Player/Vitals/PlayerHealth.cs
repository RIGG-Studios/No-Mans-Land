using System;
using System.Collections;
using System.Net.Mime;
using Fusion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class PlayerHealth : NetworkHealthHandler, INetworkInstigator, INetworkDamagable
{
    [Networked(OnChanged = nameof(OnDeadChanged))]
    public NetworkBool IsDead { get; private set; }

    [SerializeField] private Text healthText;
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private Text respawnTimeText;
    
    public UnityEvent<HitData> onDamageTaken;
    public UnityEvent onDeath;


    private const byte StartingHealth = 100;
    private const float RespawnTimer = 10f;

    public bool IsActive => true;
    public PlayerRef OwnerRef => Object.InputAuthority;

    private void Start()
    {
        Health = StartingHealth;
    }

    
    private static void OnDeadChanged(Changed<PlayerHealth> changed)
    {
        Debug.Log($"{Time.time} OnStateChanged isDead {changed.Behaviour.IsDead}");

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
        
        if (Object.HasInputAuthority)
        {
            deathPanel.SetActive(true);
            onDeath?.Invoke();
        }
    }

    public override bool Damage(HitData hitData)
    {
        RPC_Damage(hitData.Damage);
        onDamageTaken.Invoke(hitData);

        return true;
    }

    protected override void OnHealthReduced()
    {
        if (Object.HasInputAuthority)
        {
            healthText.text = "+ " + Health;
        }
    }
    

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_Damage(float damage)
    {
        Debug.Log($"{Time.time} {transform.name} took damage got {Health} left ");
        Health -= (byte)damage;
        
        if (Health <= 0)
        {
            Debug.Log($"{Time.time} {transform.name} died");
            Health = 0;
            IsDead = true;
        }
    }

    public bool ProcessHit(ref HitData hit)
    {
        return Damage(hit);
    }

    public void HitPerformed(HitData hit)
    {
        
    }
}
