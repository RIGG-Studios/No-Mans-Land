using TMPro;
using UnityEngine;

public class GameplayUI : UIComponent
{
    [SerializeField] private TextMeshProUGUI gameplayTimeText;
    [SerializeField] private GameObject objectiveStatusChangedPanel;
    [SerializeField] private TextMeshProUGUI objectiveStatusText;
    [SerializeField] private TextMeshProUGUI blueTeamScoreText;
    [SerializeField] private TextMeshProUGUI redTeamScoreText;

    private void Update()
    {
        if (Context.Session == null)
        {
            return;
        }

        if (Context.Session.Object == null)
        {
            return;
        }

        float time = Context.Session.GameplayTimer;
        
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        
        gameplayTimeText.text = string.Format("{0:0}:{1:00}", minutes, seconds);

        redTeamScoreText.text = Context.Teams.Teams[0].TeamScore.ToString();
        blueTeamScoreText.text = Context.Teams.Teams[1].TeamScore.ToString();

    }

    public void OnObjectiveWon(string id)
    {
        objectiveStatusChangedPanel.SetActive(true);
        objectiveStatusText.text = "ALLIES CAPTURED: " + "'" + id + "'";
        
        Invoke(nameof(ResetObjectivePanel), 2.0f);
    }

    public void OnObjectiveLost(string id)
    {
        objectiveStatusChangedPanel.SetActive(true);
        objectiveStatusText.text = "ENEMIES CAPTURED: " + "'" + id + "'";
        
        Invoke(nameof(ResetObjectivePanel), 2.0f);
    }

    private void ResetObjectivePanel() => objectiveStatusChangedPanel.SetActive(false);
}
