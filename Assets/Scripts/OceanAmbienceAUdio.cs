using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class OceanAmbienceAUdio : MonoBehaviour
{
    [SerializeField] private OceanAudio[] oceanAudio;
    [SerializeField] private AudioSource audioSource;

    [Serializable]
    public struct OceanAudio
    {
        public int depth;
        public AudioClip[] clips;
    }

    private NetworkPlayer _player;

    private OceanAudio _currentAudio;
    private bool _isPlaying;

    private void Awake()
    {
        _player = GetComponent<NetworkPlayer>();
    }

    private void Update()
    {
        if (!_player.Movement.InWater)
        {
            return;
        }

        if (!Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 250f))
        {
            return;
        }

        float dist = hit.distance;

        for (int i = 0; i < oceanAudio.Length; i++)
        {
            if (dist >= oceanAudio[i].depth && _currentAudio.depth != oceanAudio[i].depth)
            {
                _currentAudio = oceanAudio[i];
            }
        }

        if (_currentAudio.clips != null && !_isPlaying)
        {
            audioSource.clip = _currentAudio.clips[Random.Range(0, _currentAudio.clips.Length)];
            audioSource.Play();
            audioSource.loop = true;
            _isPlaying = true;
        }
        else
        {
      //      audioSource.Stop();
       //     audioSource.loop = false;
     //       _isPlaying = false;
        }
    }
}
