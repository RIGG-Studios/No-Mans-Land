using System;
using UnityEngine;

public class SpawnPointUI : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    private ISpawnPoint _spawnPoint;
    private NetworkSpawnHandler _spawnHandler;

    private bool _followTransform;
    
    public void Init(ISpawnPoint spawnPoint, NetworkSpawnHandler spawnHandler, bool followTransform = true)
    {
        _spawnHandler = spawnHandler;
        _spawnPoint = spawnPoint;
        _followTransform = followTransform;
    }

    private void Update()
    {
        if (!_followTransform)
        {
            return;
        }

        transform.position = _spawnPoint.Transform.position + new Vector3(0.0f, 100f, 0.0f);
    }

    public void OnClick()
    {
        _spawnHandler.OnSpawnPointSelected(_spawnPoint);
        _animator.SetTrigger("Select");
    }

    public void Reset()
    {
        _animator.SetTrigger("Hide");
    }
}
