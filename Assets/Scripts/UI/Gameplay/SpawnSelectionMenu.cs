using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSelectionMenu : UIComponent
{
    [SerializeField] private GameObject spawnUIPrefab;
    [SerializeField] private Transform canvas;
    [SerializeField] private float yOffset;
    
    public List<SpawnPointUI> _spawnUI = new();

    private NetworkSpawnHandler _spawnHandler;

    private void Awake()
    {
        _spawnHandler = FindObjectOfType<NetworkSpawnHandler>();
    }

    public override void Enable()
    {
        base.Enable();
        
        Debug.Log(_spawnHandler);
        _spawnHandler.RefreshSpawnPoints();
    }


    public void RefreshSpawnPoints(ISpawnPoint[] spawnPoints)
    {
        Debug.Log(Player.Local);
        ResetUI();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i].TeamID == Player.Local.Stats.TeamID)
            {
                SpawnUI(spawnPoints[i]);
            }
        }
    }

    private void SpawnUI(ISpawnPoint spawnPoint)
    {
        if (spawnPoint.SpawnType == SpawnPointTypes.Ship)
        {
            return;
        }

        SpawnPointUI ui = Instantiate(spawnUIPrefab, canvas).GetComponent<SpawnPointUI>();
        ui.transform.position = spawnPoint.Transform.position + new Vector3(0.0f, yOffset, 0.0f);
        
        ui.Init(spawnPoint, _spawnHandler);
        _spawnUI.Add(ui);
    }

    private void ResetUI()
    {
        for (int i = 0; i < _spawnUI.Count; i++)
        {
            Destroy(_spawnUI[i].gameObject);
        }
        
        _spawnUI.Clear();
    }
}
