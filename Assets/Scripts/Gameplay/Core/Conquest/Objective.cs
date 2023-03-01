using System;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct ObjectiveTeamProgress : INetworkStruct
{
    public int TeamID;
    public int PlayerCount;
    public float Progress;
}

public class Objective : ContextBehaviour
{
    public string objectiveName;

    [SerializeField] private float captureTime;
    [SerializeField] private ObjectiveUI objectiveUI;
    [SerializeField] private ObjectiveHeaderUI headerUI;
    [SerializeField] private Image ringImage;
    [SerializeField] private TextMeshProUGUI objectiveNameText;
    
    [Networked]
    public ObjectiveTeamProgress blueProgress { get; set; }
    
    [Networked]
    public ObjectiveTeamProgress redProgress { get; set; }
    
    [Networked]
    public int capturedTeamID { get; set; }

    private float _redCaptureTime;
    private float _blueCaptureTime;

    private bool _redCaptured;
    private bool _blueCaptured;

    public override void Spawned()
    {
        if (!Object.HasStateAuthority)
        {
            return;
        }
        
        redProgress = new ObjectiveTeamProgress()
        {
            TeamID = 1,
            PlayerCount = 0,
            Progress = 0
        };

        blueProgress = new ObjectiveTeamProgress()
        {
            TeamID = 2,
            PlayerCount = 0,
            Progress = 0
        };
    }
    
    public void OnPlayerEntered(int teamID)
    {
        //blue team
        if (teamID == 2)
        {
            ObjectiveTeamProgress newBlueProgress = blueProgress;
            newBlueProgress.PlayerCount++;
            blueProgress = newBlueProgress;
        }
        //red team
        else if (teamID == 1)
        {
            ObjectiveTeamProgress newRedProgress = redProgress;
            newRedProgress.PlayerCount++;
            redProgress = newRedProgress;
        }
    }

    public void OnPlayerLeft(int teamID)
    {
        Debug.Log(teamID);
        //blue team
        if (teamID == 2)
        {
            ObjectiveTeamProgress newBlueProgress = blueProgress;
            newBlueProgress.PlayerCount--;
            blueProgress = newBlueProgress;
        }
        //red team
        else if (teamID == 1)
        {
            ObjectiveTeamProgress newRedProgress = redProgress;
            newRedProgress.PlayerCount--;
            redProgress = newRedProgress;
        }
    }
    
    public override void FixedUpdateNetwork()
    {
        if (redProgress.PlayerCount > blueProgress.PlayerCount)
        {
            if (Object.HasStateAuthority)
            {
                if (blueProgress.Progress > 0)
                {
                    _blueCaptureTime -= Runner.DeltaTime * captureTime;
                    ObjectiveTeamProgress blueProgress = this.blueProgress;
                    blueProgress.Progress = _blueCaptureTime;
                    this.blueProgress = blueProgress;
                    return;
                }
                
                if (_redCaptureTime >= 1f && !_redCaptured)
                {
                    //red team captured objective
                    Context.Gameplay.OnObjectiveStatusChanged(2,1, objectiveName);

                    if (_blueCaptured)
                    {
                        Context.Teams.UpdateObjectives(-1, 2);
                    }
                    
                    Context.Teams.UpdateObjectives(1, 1);

                    _redCaptured = true;
                    _blueCaptured = false;
                }
                else
                {
                    _blueCaptureTime = 0.0f;
                    _redCaptureTime += Runner.DeltaTime * captureTime;
                    _redCaptureTime = Mathf.Clamp01(_redCaptureTime);
                }
                
                ObjectiveTeamProgress redProgress = this.redProgress;
                redProgress.Progress = _redCaptureTime;
                this.redProgress = redProgress;
            }
        }
        else if (blueProgress.PlayerCount > redProgress.PlayerCount)
        {
            if (Object.HasStateAuthority)
            {
                if (redProgress.Progress > 0)
                {
                    _redCaptureTime -= Runner.DeltaTime * captureTime;
                    ObjectiveTeamProgress redProgress = this.redProgress;
                    redProgress.Progress = _redCaptureTime;
                    this.redProgress = redProgress;
                    return;
                }
                
                if (_blueCaptureTime >= 1f && !_blueCaptured)
                {
                    Context.Gameplay.OnObjectiveStatusChanged(1,2, objectiveName);

                    if (_redCaptured)
                    {
                        Context.Teams.UpdateObjectives(-1, 1);
                    }
                    
                    Context.Teams.UpdateObjectives(1, 2);
                    _redCaptured = false;
                    _blueCaptured = true;
                }
                else
                {
                    _redCaptureTime = 0.0f;
                    _blueCaptureTime += Runner.DeltaTime * captureTime;
                    _blueCaptureTime = Mathf.Clamp01(_blueCaptureTime);
                }
                
                ObjectiveTeamProgress blueProgress = this.blueProgress;
                blueProgress.Progress = _blueCaptureTime;
                this.blueProgress = blueProgress;
            }
        }
    }

    public override void Render()
    {
        (float, Color) status = GetObjectiveStatus();
        
        ringImage.fillAmount = status.Item1;
        objectiveNameText.color = status.Item2;
        objectiveNameText.text = objectiveName;
        ringImage.color = status.Item2;
        
        objectiveUI.UpdateProgress(status.Item1, status.Item2);
        headerUI.UpdateProgress(status.Item1, status.Item2);
    }

    public (float, Color) GetObjectiveStatus()
    {
        float progress = 0.0f;
        Color color = new Color();

        if (blueProgress.Progress > 0)
        {
            progress = blueProgress.Progress;
            color = Context.Config.blueTeamColor;
        }
        else if (redProgress.Progress > 0)
        {
            progress = redProgress.Progress;
            color = Context.Config.redTeamColor;
        }
        else
        {
            progress = 1f;
            color = Color.white;
        }

        return (progress, color);
    }
}
    