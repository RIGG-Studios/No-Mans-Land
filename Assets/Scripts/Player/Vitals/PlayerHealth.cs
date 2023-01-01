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
    
    public UnityEvent<TakeDamageData> onDamageTaken;
    public UnityEvent<OnDeathData> onDeath;


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

        bool isDeadOld = changed.Behaviour.IsDead;

        if (isDeadCurrent)
            changed.Behaviour.OnDeath();
    }

    private void OnDeath()
    {
        onDeath?.Invoke(new OnDeathData()
        {
            Attacker = String.Empty
        });
    }

    public override void Damage(float damage, PlayerRef attackerRef)
    {
        RPC_Damage(damage);
        
        onDamageTaken.Invoke(new TakeDamageData
        {
            Damage = damage,
            Attacker = attackerRef,
            NewHealth = Health
        });
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

    public void ProcessHit(ref HitData hit)
    {
        Debug.Log("Procesed Hit Success");
        Damage(hit.Damage, hit.AttackerRef);
    }

    public void HitPerformed(HitData hit)
    {
        
    }
}
