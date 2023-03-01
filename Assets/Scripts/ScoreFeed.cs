using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreFeed : MonoBehaviour 
{

    #region Internal
    public class Score
    {
        public GameObject entity;
        public ScoreFeed.state currentState;
        public ScoreFeed.level messageLevel;
        public string text;
        public int coins;
        public float actualElapsedTime = 0;

        public float currentIndex = 0;
        public bool invisible = true;

        public Score(string _text, GameObject _gameObject, int _coins, ScoreFeed.level _level)
        {
            currentState = ScoreFeed.state.SPAWNING;
            messageLevel = _level;
            entity = _gameObject;
            coins = _coins;
            text = _text;
        }

        public void Despawn()
        {
            currentState = ScoreFeed.state.DESPAWNING;
            invisible = false;
            currentIndex = 0;
        }
    }
    public enum state
    {
        SPAWNING,
        STAYING,
        DESPAWNING
    }
    public enum level
    {
        KILL,
        INFO
    }

    List<QuedScore> queue = new List<QuedScore>();
    public class QuedScore
    {
        public ScoreFeed.level Level;
        public string info_or_Weapon;
        public string victim;
        public int coins;

        public QuedScore(string info, int _coins)
        {
            info_or_Weapon = info;
            victim = null;
            coins = _coins;
            Level = ScoreFeed.level.INFO;
        }
        public QuedScore(string weapon, string _victim, int _coins)
        {
            info_or_Weapon = weapon;
            victim = _victim;
            coins = _coins;
            Level = ScoreFeed.level.KILL;
        }
    }
    #endregion

    public GameObject ScorePrefab = null;
    public Transform Container = null;

    [Space]


	public int MaxInfoOnScreen = 7;
	public float InfoStayTime = 7f;
    public float KillSpace = 19;
    public float infoYspace = 17;

    [Space]

    public int KillFont = 14;
    public int InfoFont = 11;

    [Space]

    public Text TotalScore;


    private int TargetScore = 0;
    private int DisplayedScore = 0;

    double alpha = 0d;

    private List<Score> scores = new List<Score>();
    private List<Score> delete = new List<Score>();
    
    public void OnPlayerKilled(string _player, string _playerKilled, string _weapon)
    {
        queue.Add(new QuedScore(_weapon, _playerKilled, 100));
    }

    void Update()
    {
        UpdateScore();

        InformationLogic();

		if (Input.GetKeyDown (KeyCode.T))
        {
            Info("DOUBLE KILL", 50);
            Info("HEADSHOT BONUS", 100);
            queue.Add(new QuedScore("AK-47", "DEV-TEST", 100));
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Info("SHIP SUNK", 50);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            Info("SHIP DAMAGE", 25);
        }
	}

    private void InformationLogic()
    {
        int spawningSize = 0;

        foreach (Score score in scores)
        {
            if (score.entity.GetComponent<Text>().text.ToCharArray().Length < score.text.ToCharArray().Length)
            {
                // Making the text appear
                if (score.messageLevel == level.INFO)
                {
                    int index = score.text.ToCharArray().Length;
                    int indexToShowUp = score.entity.GetComponent<Text>().text.ToCharArray().Length;
                    score.currentIndex += index * Time.deltaTime / 5f;
                    indexToShowUp += (int)score.currentIndex;

                    string _text = "";
                    for (int _i = 0; _i <= indexToShowUp; _i++)
                    {
                        if (_i < score.text.ToCharArray().Length)
                            _text = _text + score.text.ToCharArray()[_i].ToString();
                    }
                    score.entity.GetComponent<Text>().text = _text;
                }
            }

            if (score.currentState == state.SPAWNING)
            {
                spawningSize++;
                // Making the other texts lower
                float decrease = KillSpace;
                if (score.messageLevel == level.INFO)
                    decrease = infoYspace;
                foreach (Score _scores in scores)
                {
                    if (_scores != score)
                        _scores.entity.transform.localPosition -= new Vector3(0f, decrease * Time.deltaTime * 2f, 0f);
                }

                // Making it stop after the 1/2second
                score.actualElapsedTime += Time.deltaTime * 2f;
                if (score.actualElapsedTime >= 1)
                    score.currentState = state.STAYING;
            }
            else if (score.currentState == state.STAYING)
            {
                // NOTHING
            }
            else if (score.currentState == state.DESPAWNING)
            {
                Destroy(score.entity);
                delete.Add(score);
            }
        }
        foreach (Score toDelete in delete)
        {
            scores.Remove(toDelete);
        }
        delete.Clear();

        if (spawningSize == 0 && queue.Count > 0)
        {
            if (queue[0].Level == level.INFO)
                LaterInfo(queue[0].info_or_Weapon, queue[0].coins);
            else if (queue[0].Level == level.KILL)
                LaterKill(queue[0].info_or_Weapon, queue[0].victim, queue[0].coins);

            queue.Remove(queue[0]);
        }
    }



    public void Info(string info, int coins) 
    {
        queue.Add(new QuedScore(info, coins));
    }

    private void LaterKill(string _weapon, string victim, int coins)
	{
		GameObject _score = Instantiate (ScorePrefab, Container) as GameObject;
		_score.GetComponent<Text> ().fontSize = KillFont;
		_score.GetComponent<Text> ().text = "[" + _weapon + "] <color=#FF3A00>" + victim + "</color><color=#ffffff>   +" + coins + "</color>";
        _score.transform.localPosition = Vector3.zero;
        _score.GetComponent<Animation>().Play("FeedDraw");
        
        scores.Add (new Score ("", _score, coins, level.KILL));

		StartCoroutine (Unspawn(scores[scores.Count -1], InfoStayTime));
		StartCoroutine (SetVisible (scores [scores.Count - 1]));

		if (scores.Count >= MaxInfoOnScreen)
			scores [0].Despawn ();

        AddScore(coins);
    }
    private void LaterInfo(string info, int coins)
	{
		GameObject _score = Instantiate (ScorePrefab, Container) as GameObject;
		_score.GetComponent<Text> ().fontSize = InfoFont;
        _score.GetComponent<Text>().text = "";
        _score.transform.localPosition = Vector3.zero;
        _score.GetComponent<Animation>().Play("FeedDraw");

        string _text = info + "   " + "+" + coins.ToString();
        scores.Add (new Score (_text, _score, coins, level.INFO));

		StartCoroutine (Unspawn(scores[scores.Count -1], InfoStayTime));
		StartCoroutine (SetVisible (scores [scores.Count - 1]));

		if (scores.Count >= MaxInfoOnScreen)
			scores [0].Despawn ();

        AddScore(coins);
	}

	private IEnumerator Unspawn(Score _score, float time)
	{
		yield return new WaitForSeconds (time);
		_score.Despawn ();
	}
	IEnumerator SetVisible(Score _score)
	{
		yield return new WaitForSeconds (0.5f);
		_score.invisible = false;
	}

    private void UpdateScore()
    {
        if (TargetScore != 0 && alpha < 1)  alpha += 1d * Time.deltaTime * 2f;
        if(TargetScore == 0 && alpha > 0) alpha -= 1d * Time.deltaTime * 2f;
        
        TotalScore.color = new Color(1f, 1f, 1f, (float)alpha);
        DisplayedScore = (int)Vector3.Lerp(new Vector3(DisplayedScore, 0f, 0f), new Vector3(TargetScore, 0f, 0f), Time.deltaTime * 5f).x;
        TotalScore.text = "+" + DisplayedScore;
    }

    private void DespawnTotalScore()
    {
        TargetScore = 0;
    }
    private void AddScore(int _score)
    {
        TargetScore += _score;
        CancelInvoke();
        Invoke("DespawnTotalScore", 7f);
    }
}
