using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;

public class Footstep : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private FootstepProperties[] footsteps;
    [SerializeField] private float walkDelayTime = 0.45f;
    [SerializeField] private float sprintDelayTime = 0.34f;

    [SerializeField] private AudioSource footstepAudioSource;
    
    [Serializable]
    public struct FootstepProperties
    {
        public string id;
        public AudioClip[] clips;
    }

    private NetworkPlayer _player;

    private float _footstepTime;
    private float _delayTime;

    private void Awake()
    {
        _player = GetComponent<NetworkPlayer>();
    }

    private void Update()
    {
        if (!_player.Movement.IsMoving) 
        {
            _footstepTime = 0.0f;
            return;
        }

        if (!_player.Movement.IsSwimming && !_player.Movement.IsGrounded)
        {
            _footstepTime = 0.0f;
            return;
        }

        if (_player.Movement.IsSprinting)
        {
            _delayTime = sprintDelayTime;
        }
        else
        {
            _delayTime = walkDelayTime;
        }

        _footstepTime += Time.deltaTime;

        if (_footstepTime >= _delayTime)
        {
            PlayAudio();
        }
    }

    private void PlayAudio()
    {
        if (!Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 250.0f))
        {
            return;
        }

        FootstepProperties footstep = GetFootstepAudio(hit.collider.tag);
        
        if (footstep.clips.Length <= 0)
        {
            return;
        }
        
        footstepAudioSource.PlayOneShot(footstep.clips[Random.Range(0, footstep.clips.Length)]);
        _footstepTime = 0.0f;
    }

    private FootstepProperties GetFootstepAudio(string surfaceTag)
    {
        if (_player.Movement.InWater)
        {
            return FindFootstep("Water");
        }

        if (_player.Movement.IsSwimming)
        {
            return FindFootstep("Swim");
        }

        return FindFootstep(surfaceTag);
    }

    private FootstepProperties FindFootstep(string id)
    {
        for (int i = 0; i < footsteps.Length; i++)
        {
            if (id == footsteps[i].id)
            {
                return footsteps[i];
            }
        }

        return footsteps[0];
    }
}
