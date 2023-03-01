using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    [SerializeField] private float lifeTime = 10f;
    [SerializeField] private float force;

    private float _time;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Init(Vector3 dir)
    {
        _rigidbody.AddForce(dir * force, ForceMode.Impulse);
    }
    
    private void Update()
    {
        _time += Time.deltaTime;

        if (_time >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
