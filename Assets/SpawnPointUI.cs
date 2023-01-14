using System;
using UnityEngine;

public class SpawnPointUI : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    public ISpawnPoint SpawnPoint { get; private set; }
    
    private NetworkSpawnHandler _spawnHandler;
    private bool _followTransform;
    
    public void Init(ISpawnPoint spawnPoint, NetworkSpawnHandler spawnHandler, bool followTransform = true)
    {
        _spawnHandler = spawnHandler;
        SpawnPoint = spawnPoint;
        _followTransform = followTransform;
    }

    private void Update()
    {
        if (!_followTransform)
        {
            return;
        }

        transform.position = SpawnPoint.Transform.position + new Vector3(0.0f, 100f, 0.0f);
    }

    public void OnClick()
    {
        _spawnHandler.OnSpawnPointSelected(SpawnPoint);
        _animator.SetTrigger("Select");
    }

    public void Reset()
    {
        _animator.SetTrigger("Hide");
    }
}
