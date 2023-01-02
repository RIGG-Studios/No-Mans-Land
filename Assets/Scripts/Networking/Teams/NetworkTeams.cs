using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[System.Serializable]
public struct NetworkTeam : INetworkStruct
{
    public byte TeamID;
    public int PlayerCount;


    public NetworkBool IsFull => PlayerCount >= 1;

    public NetworkTeam(byte teamID, int playerCount)
    {
        TeamID = teamID;
        PlayerCount = playerCount;
    }
}

public class NetworkTeams : ContextBehaviour
{
    [SerializeField] private int teamAmount = 4;
    [SerializeField] private bool autoFillTeams = true;

    [Networked, Capacity(16)] 
    public NetworkLinkedList<NetworkTeam> Teams { get; } = new();

    private TeamUIItem[] _teamUi = new TeamUIItem[4];

    private bool _initialized;

    public void OnEnable()
    {
        Context.Teams = this;
    }

    protected override void Awake()
    {
        base.Awake();

        _teamUi = GetComponentsInChildren<TeamUIItem>();
    }
    
    private void Init()
    {
        for (int i = 0; i < teamAmount; i++)
        {
            int id = i + 1;
            
            Teams.Add(new NetworkTeam((byte)id, 0));
        }
        
        _initialized = true;
    }

    public void SelectTeam(int i)
    {
        RPC_RequestTeamJoin(i, Runner.LocalPlayer);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestTeamJoin(int teamIndex, PlayerRef requestedPlayer)
    {
        Context.Gameplay.TryFindPlayer(requestedPlayer, out Player player);

        if (player == null)
        {
            return;
        }
        
        AddToTeam(player, teamIndex);
    }

    public void AddToTeam(Player player, int teamIndex = -1)
    {
        if (!_initialized)
        {
            Init();
        }
        
        byte teamID = 0;
        
        if (autoFillTeams)
        {
            teamID = GetBestTeam();
        }
        else
        {
            teamID = Teams[teamIndex].TeamID;
            UpdateTeamProperties(teamIndex, player.PlayerName.ToString());
        }

        player.SetStat(StatTypes.TeamID, teamID);
    }

    private byte GetBestTeam()
    {
        for (int i = 0; i < Teams.Count; i++)
        {
            if (Teams[i].IsFull)
            {
                continue;
            }

            NetworkTeam team = Teams[i];
            team.PlayerCount++;
            Teams.Set(i, team);
            return Teams[i].TeamID;
        }
        return 0;
    }

    private void UpdateTeamProperties(int teamIndex, string playerName)
    {
        NetworkTeam team = Teams[teamIndex];
        team.PlayerCount++;
        Teams.Set(teamIndex, team);
    }
}
